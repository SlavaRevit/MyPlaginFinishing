using System;
using Autodesk.Revit.DB;
using SelectionFilter.Extensions.BoundingBoxes;
using SelectionFilter.Extensions.Solids;
using SelectionFilter.Options;


namespace SelectionFilter.BoundingBoxVisualizations
{

    public class FacesBoundingBoxVisualization : IBoundingBoxVisualization
    {
        public void VisualizeIn(BoundingBoxXYZ boundingBox, Document document, Action<BoundingBoxVisualizationOption>? configOptions = null)
        {
            var options = new BoundingBoxVisualizationOption();
            configOptions?.Invoke(options);
            var curveLoops = boundingBox.GetCurveLoopOfAllSides(options.ApplyTransform);
            foreach (var curveLoop in curveLoops)
            {
                curveLoop.VisualizeIn(document);
            }

        }
    }
}