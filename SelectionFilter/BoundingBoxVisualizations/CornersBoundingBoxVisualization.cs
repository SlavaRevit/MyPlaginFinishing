using System;
using Autodesk.Revit.DB;
using SelectionFilter.Extensions;
using SelectionFilter.Extensions.BoundingBoxes;
using SelectionFilter.Options;



namespace SelectionFilter.BoundingBoxVisualizations
{

    public class CornersBoundingBoxVisualization : IBoundingBoxVisualization
    {
        public void VisualizeIn(BoundingBoxXYZ boundingBox, Document document, Action<BoundingBoxVisualizationOption>? configOptions = null)
        {
            var options = new BoundingBoxVisualizationOption();
            configOptions?.Invoke(options);
            var cornerVertices = boundingBox
                .GetCornerVertices(options.ApplyTransform);
            cornerVertices.VisualizeIn(document);
        }
    }
}