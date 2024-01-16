using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorsAndPointsNew.Extension
{
	public static class DocumentExtensions
	{
		/// <summary>
		/// This method used to create direct shapes in a document
		/// </summary>
		/// <param name="document">Current document</param>
		/// <param name="geometryObject">Geometry objects to visualize</param>
		/// <param name="categoryId"> Category Id </param>
		/// <returns></returns>
		public static DirectShape CreateDirectShape(this Document document,
			List<GeometryObject> geometryObject,
			BuiltInCategory builtinCategory = BuiltInCategory.OST_GenericModel)
		{
			var directShape = DirectShape.CreateElement(document, new ElementId(builtinCategory));
			directShape.SetShape(geometryObject);
			return directShape;

		}

	}
}
