using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SelectionFilter.BoundingBoxVisualizations;
using SelectionFilter.Builders;
using SelectionFilter.Enums;
using SelectionFilter.Extensions;
using SelectionFilter.Extensions.BoundingBoxes;
using SelectionFilter.Extensions.Solids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectionFilter
{
	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class VisualizeDoorSolids : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			var uiapp = commandData.Application;
			var uidoc = uiapp.ActiveUIDocument;
			var app = uiapp.Application;
			var doc = uidoc.Document;

			var walls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().ToElements().ToList();
			var furniture = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Furniture).WhereElementIsNotElementType().ToElements().ToList();
			var doors = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Doors).WhereElementIsNotElementType().ToElements().ToList();

            //double length = 3;
            //double minX = -length / 2 * 2;
            //double maxX = length + minX;

            //var transform = Transform.CreateTranslation(XYZ.BasisX * 5)
            //	.Multiply(Transform.CreateRotation(XYZ.BasisZ, 30.0.ToRadians()));

            //var box = new BoundingBoxXYZ()
            //{
            //	Min = new XYZ(-1, -1, -1),
            //	Max = new XYZ(1, -1, -1),
            //	Transform = transform,
            //};

            //var pt1 = box.Min;
            //var pt2 = pt1.MoveAlongVector(XYZ.BasisX * 2);
            //var pt3 = pt2.MoveAlongVector(XYZ.BasisY * 2);
            //var pt4 = pt3.MoveAlongVector(XYZ.BasisX * -2);
            //var curveLoop = CurveLoop.Create(new List<Curve>
            //{
            //	Line.CreateBound(pt1,pt2),
            //	Line.CreateBound(pt2,pt3),
            //	Line.CreateBound(pt3,pt4),
            //	Line.CreateBound(pt4,pt1),

            //});

            //var solid = GeometryCreationUtilities
            //	.CreateExtrusionGeometry(new List<CurveLoop> { curveLoop }, XYZ.BasisZ, 2)
            //	.CreateTransformed(transform);

            BoundingBoxXYZ boundingBox = new BoundingBoxBuilder()
            .OfLength(5)
            .OfWidth(5)
            .OfHeight(5)
            .WithTransform(Transform.CreateTranslation(new XYZ(5, 0, 0)))
            .Build();

            //var visualization = new SolidBoundingBoxVisualization();

            //var builder = new BoundingBoxVisualizationBuilder()
            //	.Add(visualization)
            //	.Build();




            doc.Run(() =>
			{
				//boundingBox.Align(Side.Length, Alignment.Right);
				//boundingBox.Align(Side.Width, Alignment.Bottom);
				//boundingBox.VisualizeIn(doc, visualization, Enums.ApplyTransform.Yes);

				//box.Transform.VisualizeIn(doc);
				//solid.VisualizeIn(doc);
				foreach (var door in doors)
				{
                    var doorTransform = (door as Instance).GetTransform();

					var boundingBoxDoor = door.get_BoundingBox(doc.ActiveView);
					

					boundingBoxDoor.VisualizeIn(doc, new SolidBoundingBoxVisualization(), ApplyTransform.Yes);
                    

				}

			});

			return Result.Succeeded;

		}
	}
}
