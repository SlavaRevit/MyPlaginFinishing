using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using SelectionFilter.Enums;
using SelectionFilter.Extensions.BoundingBoxes;
using SelectionFilter.Interfaces;

namespace SelectionFilter.SoolidConverters
{
	public class CuboidBoundingBoxSolidConverter : IBoundingBoxSolidConverter
	{
		public Solid Convert(BoundingBoxXYZ boundingBox, Action<BoundingBoxSolidConverterOptions>? configOptions = null)
		{
			var options = new BoundingBoxSolidConverterOptions();
			configOptions?.Invoke(options);
			var curveLoop = boundingBox.GetCurveLoop(FaceSide.Bottom, options.ApplyTransform);
			var solid = GeometryCreationUtilities.CreateExtrusionGeometry(
				new List<CurveLoop>() { curveLoop },
				boundingBox.Transform.BasisZ,
				boundingBox.CalculateDimension().Height);
			return solid;
		}
	}
}

