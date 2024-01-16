using Autodesk.Revit.DB;
using SelectionFilter.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectionFilter.Extensions
{
	public static class SideExtensions
	{

		public static XYZ GetCorrespondingVector(this Side side) => side switch
		{
			Side.Length => XYZ.BasisX,
			Side.Width => XYZ.BasisY,
			Side.Height => XYZ.BasisZ,
			_ => throw new InvalidOperationException($"Cannot use ${side.ToString()}")
		};

	}
}
