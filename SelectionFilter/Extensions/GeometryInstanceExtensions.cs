using Autodesk.Revit.DB;
using SelectionFilter.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectionFilter.Extensions.Solids
{
	public static class GeometryInstanceExtensions
	{
		public static IEnumerable<T> ExtractGeometries<T>(
					this GeometryInstance geometryInstance,
					GeometryRepresentation geometryRepresentation) where T : GeometryObject
		{
			var familyGeometryElement = (geometryRepresentation == GeometryRepresentation.Symbol)
				? geometryInstance.SymbolGeometry
				: geometryInstance.GetInstanceGeometry();
			foreach (var geometryObject in familyGeometryElement.ExtractRootGeometries<T>(geometryRepresentation))
			{
				yield return geometryObject;
			}
		}

	}
}
