using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectionFilter.Models
{
	public ref struct AlignmentResult
	{
		public XYZ RotationAxis { get; }
		public double Angle { get; }

		public AlignmentResult(XYZ rotationAxis, double angle)
		{
			RotationAxis = rotationAxis;
			Angle = angle;
		}
	}
}
