using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;


namespace SelectionFilter.Extensions.SelectionExtensions
{
	public class BothDocumentOption : IPickElementsOption
	{
		public List<Element> PickElements(UIDocument uIDocument, Func<Element, bool> validateElement)
		{
			var doc = uIDocument.Document;
			var references = uIDocument.Selection.PickObjects(ObjectType.PointOnElement, 
				SelectionFilterFactory.CreateLinkableSelectionFilter(doc, validateElement));
			var elements = new List<Element>();
			foreach( var r in references)
			{
				if (doc.GetElement(r.ElementId) is  RevitLinkInstance linkInstance)
				{
					var element = linkInstance.GetLinkDocument().GetElement(r.LinkedElementId);
					elements.Add(element);
				}
				else
				{
					elements.Add(doc.GetElement(r.ElementId));
				}
			}
			return elements;
		}
	}
}
