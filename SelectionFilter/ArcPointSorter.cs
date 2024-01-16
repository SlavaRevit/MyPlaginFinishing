using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectionFilter
{
	public class ArcPointSorter
	{

		private XYZ center;
		private bool clockwise;
		private XYZ normal;
		private double startAngle;
		private XYZ startPoint;
		public ArcPointSorter(Arc arc, XYZ center, XYZ normal, double startAngle, bool clockwise = true) 
		{
			this.center = center;
			this.clockwise = clockwise;
			this.normal = normal;
			this.startAngle = startAngle;
			this.startPoint = arc.GetEndPoint(0);
		}	

		public IList<XYZ> SortPoints(List<XYZ> points)
		{
			//return points.OrderBy(p => GetAngle(p)).ToList();
			// Calculate start angle
			double startAngle = GetAngle(startPoint);

			return points.OrderBy(p => NormalizeAngle(GetAngle(p) - startAngle)).ToList();
		}

		private double GetAngle(XYZ point)
		{
			double angle = Math.Atan2(point.Y - center.Y, point.X - center.X) - startAngle;

			// Normalize angle to be between 0 and 2*PI
			if (angle < 0)
				angle += 2 * Math.PI;

			// Adjust for clockwise/counterclockwise based on normal vector
			if ((normal.Z < 0 && clockwise) || (normal.Z > 0 && !clockwise))
				angle = 2 * Math.PI - angle;

			return angle;
		}

		private double NormalizeAngle(double angle)
		{
			// Normalize angle to be between 0 and 2*PI
			while (angle < 0)
				angle += 2 * Math.PI;
			while (angle >= 2 * Math.PI)
				angle -= 2 * Math.PI;

			return angle;
		}
	}
}
