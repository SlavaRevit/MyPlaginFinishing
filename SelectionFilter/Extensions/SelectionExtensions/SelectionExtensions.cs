using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Autodesk.Revit.DB.SpecTypeId;


namespace SelectionFilter.Extensions.SelectionExtensions
{
	/// <summary>
	/// This class is used for creating extensions methods for Selecting elements in Revit UI
	/// </summary>
	public static class SelectionExtensions
	{
		public static List<Element> PickElements(this UIDocument uIDocument, Func<Element, bool> validateElement, IPickElementsOption pickElementsOption)
		{
			return pickElementsOption.PickElements(uIDocument, validateElement);

		}

	}
}
