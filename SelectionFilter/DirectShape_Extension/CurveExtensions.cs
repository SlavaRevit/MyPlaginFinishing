using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorsAndPointsNew.Extension
{
	public static class CurveExtensions
	{

		public static void VisualizeCurve(this Curve curve, Document document)
		{
			document.CreateDirectShape(new List<GeometryObject>() { curve });
		}
		


	}
}
