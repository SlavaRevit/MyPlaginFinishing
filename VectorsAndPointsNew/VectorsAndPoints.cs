using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorsAndPointsNew.Extension;
//using SelectionFilter.Extensions.SelectionExtensions;


namespace VectorsAndPointsNew
{

	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class VectorsAndPoints : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			var uiapp = commandData.Application;
			var uidoc = uiapp.ActiveUIDocument;
			var app = uiapp.Application;
			var doc = uidoc.Document;

			//var selectedElement = uidoc.PickElements(e => e is FamilyInstance, PickElementsOptionFactory.CreateCurrentDocumentOption).First();
			//var familyPoint = (selectedElement.Location as LocationPoint).Point;
			var FirstVector = new XYZ(2, 1.5, 0).Normalize();
			var basizZ = XYZ.BasisZ;
			var basiz = XYZ.Zero;

			var perpendicularVector = FirstVector.CrossProduct(basizZ);
			using (var transaction = new Transaction(doc, "create shapes"))
			{
				transaction.Start();
				//ElementTransformUtils.MoveElement(doc, selectedElement.Id, vector);
				//vector.VisualizeAsLine(doc);
				FirstVector.AsCurve().VisualizeCurve(doc);
				basizZ.AsCurve().VisualizeCurve(doc);
				perpendicularVector.AsCurve().VisualizeCurve(doc);
				transaction.Commit();
			}
			



			return Result.Succeeded;
		}
	}
}
