using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;

namespace SelectionFilter.Extensions.SelectionExtensions
{
	public abstract class BaseSelectionFilter : ISelectionFilter
	{

		protected readonly Func<Element, bool> _validateElement;
		protected BaseSelectionFilter(Func<Element, bool> validateElement)
		{
			_validateElement = validateElement;
		}

		public abstract bool AllowElement(Element elem);
		public abstract bool AllowReference(Reference reference, XYZ position);
	}
}
