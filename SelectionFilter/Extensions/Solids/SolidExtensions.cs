using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectionFilter.Extensions.Solids
{
	public static class SolidExtensions
	{

		public static Solid CreateTransformed(this Solid solid, Transform transform)
		{
			return SolidUtils.CreateTransformed(solid, transform);
		}

	}
}
