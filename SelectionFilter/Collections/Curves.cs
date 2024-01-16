using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectionFilter.Collections
{
	public class Curves : List<Curve>
	{

        public Curves()
        {
                
        }

        public Curves(IEnumerable<Curve> curves) : base(curves)
        {
            
        }

    }
}
