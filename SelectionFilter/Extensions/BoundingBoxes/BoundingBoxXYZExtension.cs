using Autodesk.Revit.DB;
using SelectionFilter.BoundingBoxVisualizations;
using SelectionFilter.Collections;
using SelectionFilter.Enums;
using SelectionFilter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorsAndPointsNew.Extension;

namespace SelectionFilter.Extensions.BoundingBoxes
{
	public static class BoundingBoxXYZExtension
	{

		public static void MoveBy(this BoundingBoxXYZ boundingBox, XYZ translation)
		{
			if (translation is null) {  throw new ArgumentNullException(nameof(translation)); }
			var transform = boundingBox.Transform;
			var origin = transform.Origin.MoveAlongVector(translation);
			transform.Origin = origin;
			boundingBox.Transform = transform;
		}

		public static void ChangeOrigin(this BoundingBoxXYZ boundingBox, XYZ origin)
		{
			var vectorToMoveBy = boundingBox.Transform.Origin.ToVector(origin);
			boundingBox.MoveBy(vectorToMoveBy);
		}

		public static void ChangePlacement(this BoundingBoxXYZ boundingBox, XYZ origin, Transform orientation)
		{
			var clonedTransform = orientation.Clone();
			clonedTransform.Origin = origin;
			boundingBox.Transform = clonedTransform;
		}
		public static void MoveToGlobalOrigin(this BoundingBoxXYZ boundingBox)
		{
			boundingBox.MoveBy(boundingBox.GetCenter().Negate());
		}


		public static XYZ GetCenter(this BoundingBoxXYZ boundingBox, ApplyTransform applyTransform = ApplyTransform.No)
		{
			if (boundingBox == null)  throw new ArgumentNullException(nameof(boundingBox)); 
			var center = boundingBox.Max.MoveAlongVector(boundingBox.Min).Multiply(0.5);
			return (applyTransform == ApplyTransform.No) 
				? center 
				: boundingBox.Transform.OfPoint(center);
		}


		public static CornerVertices GetCornerVertices(this BoundingBoxXYZ box, ApplyTransform applyTransform = ApplyTransform.Yes)
		{
			if (box == null) throw new ArgumentNullException(nameof(box));
			var vertices = Enumerable.Range(0, 2)
				.Select(i =>
				{
					var vertex = box.get_Bounds(i);
					return (applyTransform == ApplyTransform.Yes)
					? box.Transform.OfPoint(vertex)
					: vertex;
				}).ToArray();
			return new CornerVertices(vertices[0], vertices[1]);
		}


		public static BoundingBoxXYZ Merge(this IEnumerable<BoundingBoxXYZ> boxes)
		{
			if (boxes == null) throw new ArgumentNullException(nameof(boxes));
			return boxes
				.Aggregate((previous, next) => previous.MergeWith(next));
		}

		public static BoundingBoxXYZ MergeWith(this BoundingBoxXYZ fromBoundingBox, BoundingBoxXYZ toBoundingBox)
		{
			if (fromBoundingBox == null) throw new ArgumentNullException(nameof(fromBoundingBox));
			if (toBoundingBox == null) throw new ArgumentNullException(nameof(toBoundingBox));
			return CreateBoundingBoxByOutermostCorners(fromBoundingBox, toBoundingBox);
		}

		public static BoxDimension CalculateDimension(this BoundingBoxXYZ boundingbox)
		{
			var length = boundingbox.CalculateSideDimension(Side.Length);
			var width = boundingbox.CalculateSideDimension(Side.Width);
			var height = boundingbox.CalculateSideDimension(Side.Height);
			return new BoxDimension(length, width, height);
		}

		public static double CalculateSideDimension(this BoundingBoxXYZ boundingBox, Side side)
		{
			return boundingBox.Min.MeasureDistanceAlongVector(boundingBox.Max, side.GetCorrespondingVector());
		}


		private static BoundingBoxXYZ CreateBoundingBoxByOutermostCorners(BoundingBoxXYZ fromBoundingBox,
		BoundingBoxXYZ toBoundingBox)
		{
			var minX = Math.Min(fromBoundingBox.Min.X, toBoundingBox.Min.X);
			var minY = Math.Min(fromBoundingBox.Min.Y, toBoundingBox.Min.Y);
			var minZ = Math.Min(fromBoundingBox.Min.Z, toBoundingBox.Min.Z);

			var maxX = Math.Max(fromBoundingBox.Max.X, toBoundingBox.Max.X);
			var maxY = Math.Max(fromBoundingBox.Max.Y, toBoundingBox.Max.Y);
			var maxZ = Math.Max(fromBoundingBox.Max.Z, toBoundingBox.Max.Z);

			var newBoundingBox = new BoundingBoxXYZ
			{
				Min = new XYZ(minX, minY, minZ),
				Max = new XYZ(maxX, maxY, maxZ)
			};
			return newBoundingBox;
		}

		public static void SetLength(this BoundingBoxXYZ boundingBox, double value, Alignment alignment = Alignment.Center)
		{
			boundingBox.SetDimension(value, Side.Length, alignment);
		}

		public static void SetWidth(this BoundingBoxXYZ boundingBox, double value, Alignment alignment = Alignment.Center)
		{
			boundingBox.SetDimension(value, Side.Width, alignment);
		}

		public static void SetHeight(this BoundingBoxXYZ boundingBox, double value, Alignment alignment = Alignment.Bottom)
		{
			boundingBox.SetDimension(value, Side.Height, alignment);
		}
		public static void SetDimension( this BoundingBoxXYZ boundingBox, BoxDimension boxDimension)
		{
			boundingBox.SetLength(boxDimension.Length);
			boundingBox.SetWidth(boxDimension.Width);
			boundingBox.SetHeight(boxDimension.Height);
		}

		public static void Align(this BoundingBoxXYZ boundingBox, Side side, Alignment alignment)
		{
			var currentSideValue = boundingBox.CalculateSideDimension(side);
			boundingBox.SetDimension(currentSideValue, side, alignment);
		}

		public static BoundingBoxXYZ AlignCloned(this BoundingBoxXYZ boundingBox, Side side, Alignment alignment)
		{
			var clonedBox = boundingBox.Clone();
			var currentSideValue = clonedBox.CalculateSideDimension(side);
			clonedBox.SetDimension( currentSideValue, side, alignment);
			return clonedBox;
		}

		public static BoundingBoxXYZ Clone(this BoundingBoxXYZ boundingBox)
		{
			var outputBoundingBox = new BoundingBoxXYZ()
			{
				Min = boundingBox.Min,
				Max = boundingBox.Max,
				Transform = boundingBox.Transform
			};
			return outputBoundingBox;
		}


		private static void SetDimension(this BoundingBoxXYZ boundingBox, double value, Side side, Alignment alignment)
		{
			if (value <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}
			var alingmentFactor = (int)alignment;
			var sideIndex = (int)side;
			var minCoordinates = boundingBox.Min.GetCoordinates();
			var maxCoordinates = boundingBox.Max.GetCoordinates();
			var minValue = -value / 2 * alingmentFactor;
			var maxValue = value + minValue;
			minCoordinates[sideIndex] = minValue;
			maxCoordinates[sideIndex] = maxValue;
			boundingBox.Min = new XYZ(minCoordinates[0], minCoordinates[1], minCoordinates[2]);
			boundingBox.Max = new XYZ(maxCoordinates[0], maxCoordinates[1], maxCoordinates[2]);

		}

		private static BoundingBoxXYZ CreateBoundingBoxByOutermostCurners(BoundingBoxXYZ fromBoundingBox, BoundingBoxXYZ toBoundingBox)
		{
			var minX = Math.Min(fromBoundingBox.Min.X, toBoundingBox.Min.X);
			var minY = Math.Min(fromBoundingBox.Min.Y, toBoundingBox.Min.Y);
			var minZ = Math.Min(fromBoundingBox.Min.Z, toBoundingBox.Min.Z);

			var maxX = Math.Min(fromBoundingBox.Max.X, toBoundingBox.Max.X);
			var maxY = Math.Min(fromBoundingBox.Max.Y, toBoundingBox.Max.Y);
			var maxZ = Math.Min(fromBoundingBox.Max.Z, toBoundingBox.Max.Z);

			var newBoundingBox = new BoundingBoxXYZ
			{
				Min = new XYZ(minX, minY, minZ),
				Max = new XYZ(maxX, maxY, maxZ)
			};
			return newBoundingBox;

		}
		public static void VisualizeIn(
			   this BoundingBoxXYZ boundingBox,
			   Document document,
			   IBoundingBoxVisualization boundingBoxVisualization,
			   ApplyTransform applyTransform = ApplyTransform.No)
		{
			boundingBoxVisualization.VisualizeIn(
				boundingBox,
				document,
				options => options.ApplyTransform = applyTransform);
		}

	}
}
