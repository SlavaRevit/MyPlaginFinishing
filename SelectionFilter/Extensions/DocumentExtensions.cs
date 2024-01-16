using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SelectionFilter.Extensions.Solids
{
	public static class DocumentExtensions
	{
		public static DirectShape CreateDirectShape(
					this Document document,
					IEnumerable<GeometryObject> geometryObjects,
					BuiltInCategory builtInCategory = BuiltInCategory.OST_GenericModel)
		{
			var directShape = DirectShape.CreateElement(document, new ElementId(builtInCategory));
			directShape.SetShape(geometryObjects.ToList());
			return directShape;
		}

		public static DirectShape CreateDirectShape(
					this Document document,
					GeometryObject geometryObject,
					BuiltInCategory builtInCategory = BuiltInCategory.OST_GenericModel)
		{
			var directShape = DirectShape.CreateElement(document, new ElementId(builtInCategory));
			directShape.SetShape(new List<GeometryObject>() { geometryObject });
			return directShape;
		}


		public static void Run(this Document document,Action action, string prompt = "Default transaction name")
		{
			using (var tr = new Transaction(document, prompt))
			{
				tr.Start();
				action.Invoke();
				tr.Commit();
			};
		}


	}
}
