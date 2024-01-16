using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectionFilter.Extensions.Solids
{
	public static class GeometryObjectExtensions
	{

		public static void VisualizeIn(this GeometryObject geometryObject, Document document)
		{
			document.CreateDirectShape(geometryObject);
		}
		public static void VisualizeIn(this IEnumerable<GeometryObject> geometries, Document document)
		{
			document.CreateDirectShape(geometries);
		}

	}
}
