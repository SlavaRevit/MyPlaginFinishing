using System;
using Autodesk.Revit.DB;
using SelectionFilter.Extensions.Solids;
using SelectionFilter.Options;
using SelectionFilter.SoolidConverters;


namespace SelectionFilter.BoundingBoxVisualizations
{

    public class SolidBoundingBoxVisualization : IBoundingBoxVisualization
    {
        public void VisualizeIn(BoundingBoxXYZ boundingBox, Document document, Action<BoundingBoxVisualizationOption>? configOptions = null)
        {
            var options = new BoundingBoxVisualizationOption();
            configOptions?.Invoke(options);
            var solidToVisualize = new CuboidBoundingBoxSolidConverter()
                .Convert(boundingBox,
                    converterOptions => converterOptions.ApplyTransform = options.ApplyTransform);
            solidToVisualize.VisualizeIn(document);
        }
    }
}