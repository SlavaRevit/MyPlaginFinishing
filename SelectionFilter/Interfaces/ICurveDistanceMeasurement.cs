using Autodesk.Revit.DB;

namespace SelectionFilter.Interfaces
{
	public interface ICurveDistanceMeasurement
	{
		double Measure(Curve fromCurve, Curve toCurve);
	}
}

