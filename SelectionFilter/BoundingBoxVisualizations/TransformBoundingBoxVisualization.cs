using System;
using Autodesk.Revit.DB;
using SelectionFilter.Extensions;
using SelectionFilter.Options;


namespace SelectionFilter.BoundingBoxVisualizations
{

    public class TransformBoundingBoxVisualization : IBoundingBoxVisualization
    {
        public void VisualizeIn(BoundingBoxXYZ boundingBox, Document document, Action<BoundingBoxVisualizationOption>? configOptions = null)
        {
            boundingBox.Transform.VisualizeIn(document);
        }
    }

}