using Autodesk.Revit.DB;

namespace SelectionFilter
{
	public interface IVectorToTransformAlignment
	{
		Transform Align(XYZ vector);
	}
}

