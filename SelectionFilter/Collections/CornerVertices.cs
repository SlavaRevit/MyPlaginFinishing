using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectionFilter.Collections
{
	public class CornerVertices : List<XYZ>
	{

        public XYZ Left { get; }
        public XYZ Right { get; }

        public CornerVertices(XYZ left, XYZ right) : base(new[] {left, right})
        {
            Left = left;
            Right = right;
        }
    }
}
