using Autodesk.Revit.DB;
using System;

namespace SelectionFilter.Extensions.SelectionExtensions
{
	public class LinkableSelectionFilter : BaseSelectionFilter
	{
		private readonly Document _doc;

		public LinkableSelectionFilter(Document doc,
			Func<Element,bool> validateElement) : base(validateElement)
		{
			_doc = doc;
		}

		public override bool AllowElement(Element elem) => true;

		public override bool AllowReference(Reference reference, XYZ position)
		{
			if (_doc.GetElement(reference.ElementId) is RevitLinkInstance linkInstance)
			{
				var element = linkInstance.GetLinkDocument().GetElement(reference.LinkedElementId);
				return _validateElement(element);
			}
			else
			{
				return _validateElement(_doc.GetElement(reference.ElementId));
			}
			
		}
	}
}
