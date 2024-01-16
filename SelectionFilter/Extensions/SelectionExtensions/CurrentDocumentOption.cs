using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;


namespace SelectionFilter.Extensions.SelectionExtensions
{
	public class CurrentDocumentOption : IPickElementsOption
	{
		public List<Element> PickElements(UIDocument uIDocument, Func<Element, bool> validateElement)
		{
			return uIDocument.Selection.PickObjects(ObjectType.Element,
				SelectionFilterFactory.CreateElementSelectionFilter(validateElement)).Select(x => uIDocument.Document.GetElement(x.ElementId)).ToList();
		}
	}
}
