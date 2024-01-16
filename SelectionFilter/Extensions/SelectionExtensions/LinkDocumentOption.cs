using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;


namespace SelectionFilter.Extensions.SelectionExtensions
{
	public class LinkDocumentOption : IPickElementsOption
	{
		public List<Element> PickElements(UIDocument uIDocument, Func<Element, bool> validateElement)
		{
			var doc = uIDocument.Document;
			var references = uIDocument.Selection.PickObjects(ObjectType.LinkedElement,
				SelectionFilterFactory.CreateLinkableSelectionFilter(doc, validateElement));
			var elements = references.Select(r => (doc.GetElement(r.ElementId) as RevitLinkInstance)?.GetLinkDocument().GetElement(r.LinkedElementId)).ToList();

			return elements;		
		}
	}
}
