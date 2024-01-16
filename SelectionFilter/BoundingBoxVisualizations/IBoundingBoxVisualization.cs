using System;
using Autodesk.Revit.DB;
using SelectionFilter.Options;

namespace SelectionFilter.BoundingBoxVisualizations
{

	public interface IBoundingBoxVisualization
	{
		void VisualizeIn(BoundingBoxXYZ boundingBox, Document document, Action<BoundingBoxVisualizationOption>? configOptions = null);
	}
}