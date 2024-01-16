using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;


namespace SelectionFilter.Extensions.SelectionExtensions
{
	public interface IPickElementsOption
	{
		List<Element> PickElements(UIDocument uIDocument, Func<Element, bool> validateElement);
	}
}
