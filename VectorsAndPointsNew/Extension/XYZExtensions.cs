using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorsAndPointsNew.Extension
{
	public static class XYZExtensions
	{
		public static void Visualize(this XYZ point, Document document)
		{
			document.CreateDirectShape(new List<GeometryObject>() { Point.Create(point) });
		}

		public static Curve AsCurve(this XYZ vector, XYZ origin = null, double? length = null)
		{
			origin ??= XYZ.Zero;
			length ??= vector.GetLength();
			return Line.CreateBound(
				origin, 
				origin.MoveAlongVector(vector.Normalize(), length.GetValueOrDefault()));
		}

		public static XYZ MoveAlongVector(this XYZ pointToMove, XYZ vector, double distance) => pointToMove.Add(vector * distance);

		public static XYZ MoveAlongVector(this XYZ pointToMove, XYZ vector) => pointToMove.Add(vector);


		public static void VisualizeAsPoint(this XYZ point, Document document)
		{

			document.CreateDirectShape(new List<GeometryObject> { Point.Create(point) });
		}

		public static Line VisualizeAsLine(this XYZ vector, Document document, XYZ origin = null)
		{
			origin ??= XYZ.Zero;
			//var endPoint = origin + vector;
			var line = Line.CreateBound(origin, vector);
			document.CreateDirectShape(new List<GeometryObject> { line });
			return line;
		}

		public static XYZ ToNormalizedVector(this Curve curve)
		{
			return (curve.GetEndPoint(1) - curve.GetEndPoint(0)).Normalize() ;
		}

	}
}
