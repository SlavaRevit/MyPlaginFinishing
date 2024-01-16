using SelectionFilter.Enums;

namespace SelectionFilter.Builders
{
	public class SideSpecification
	{
		public double Value { get; }
		public Alignment Alignment { get; }

		public SideSpecification(double value, Alignment alignment)
		{
			Value = value;
			Alignment = alignment;
		}
	}
}

