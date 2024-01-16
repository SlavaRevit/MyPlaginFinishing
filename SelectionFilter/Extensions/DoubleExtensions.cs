using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectionFilter.Extensions
{
	public static class DoubleExtensions
	{
		public static double ToRadians(this double degree)
		{
			return degree * (Math.PI / 180.0);
		}

		public static double ToDegrees(this double radians)
		{
			return radians * (180.0 / Math.PI);
		}

		public static bool IsAlmostEqualTo(this double firstValue, double secondValue, double tolerance = 0.0001)
		{
			return Math.Abs(firstValue - secondValue) < tolerance;
		}
	}
}
