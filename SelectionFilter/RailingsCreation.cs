using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectionFilter
{
	public static class RailingsCreation
	{
		public static void filterOfPoints(Document document,List<XYZ> pointsStartEnd, List<XYZ> pointsDoors, List<XYZ> allPoints)
		{
			var profile = new List<Curve>();
			for (int i = 0; i < allPoints.Count; i++)
			{
				var positionOfPoint = allPoints[i];
				for (int j = 0; j < pointsDoors.Count; j++)
				{

					var positionOfDoorPoint = pointsDoors[j];
					if (positionOfPoint == positionOfDoorPoint)
					{
						//TaskDialog.Show("Point position", positionOfPoint.ToString());
						continue;
					}
					VisualizeLine(document, positionOfPoint, positionOfDoorPoint);
					
				}
				
				
				
			}
		}

		public static void VisualizeLine(Document document, XYZ startPoint, XYZ endpoint)
		{
			var line = Line.CreateBound(startPoint, endpoint);
			var showLine = document.Create.NewModelCurve(line, SketchPlane.Create(document,
				Plane.CreateByNormalAndOrigin(XYZ.BasisZ, new XYZ(0, 0, 0))));
		}

		
	}
}
