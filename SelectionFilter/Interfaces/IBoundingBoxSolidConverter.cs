using System;
using Autodesk.Revit.DB;

namespace SelectionFilter.Interfaces
{
	public interface IBoundingBoxSolidConverter
	{
		Solid Convert(BoundingBoxXYZ boundingBox, Action<BoundingBoxSolidConverterOptions>? configOptions = null);
	}
}

