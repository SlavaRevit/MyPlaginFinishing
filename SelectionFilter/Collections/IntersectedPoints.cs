using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace SelectionFilter.Collections
{
	public class IntersectedPoints : List<XYZ>
	{
		public IntersectedPoints() { }
		public IntersectedPoints(IEnumerable<XYZ> points) : base(points)
		{

		}
	}
}

