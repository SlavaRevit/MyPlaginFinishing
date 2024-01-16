using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectionFilter.Collections
{
	public class Vertices : List<XYZ>
	{
        public Vertices()
        {
            
        }
        public Vertices(IEnumerable<XYZ> vertices) : base(vertices)
        {
            
        }

    }
}
