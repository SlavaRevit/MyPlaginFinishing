using Autodesk.Revit.DB;
using System;


namespace SelectionFilter.Extensions.SelectionExtensions
{
	public static class SelectionFilterFactory
	{
		public static ElementSelectionFilter CreateElementSelectionFilter(Func<Element, bool> validateElement)
		{
			return new ElementSelectionFilter(validateElement);
		}
		public static LinkableSelectionFilter CreateLinkableSelectionFilter(Document doc , Func<Element,bool> validateElement)
		{
			return new LinkableSelectionFilter(doc, validateElement);
		}
	}
}
