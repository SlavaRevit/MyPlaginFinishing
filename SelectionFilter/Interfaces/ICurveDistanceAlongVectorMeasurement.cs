using Autodesk.Revit.DB;

namespace SelectionFilter.Interfaces
{
	public interface ICurveDistanceAlongVectorMeasurement
	{
		double Measure(Curve fromCurve, Curve toCurve, XYZ vectorToMeasureBy);
	}
}

