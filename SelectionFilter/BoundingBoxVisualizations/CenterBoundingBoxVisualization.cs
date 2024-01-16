using System;
using Autodesk.Revit.DB;
using SelectionFilter.BoundingBoxVisualizations;
using SelectionFilter.Extensions;
using SelectionFilter.Extensions.BoundingBoxes;
using SelectionFilter.Options;


namespace SelectionFilter.BoundingBoxVisualizations {
	public class CenterBoundingBoxVisualization : IBoundingBoxVisualization
	{
		public void VisualizeIn(BoundingBoxXYZ boundingBox, Document document, Action<BoundingBoxVisualizationOption>? configOptions = null)
		{
			var options = new BoundingBoxVisualizationOption();
			configOptions?.Invoke(options);
			boundingBox
				.GetCenter(options.ApplyTransform)
				.VisualizeIn(document);
		}
	}
}

