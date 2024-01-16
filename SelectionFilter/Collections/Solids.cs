using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectionFilter.Collections
{
	public class Solids : List<Solid>
	{

        public Solids()
        {
            
        }

        public Solids(IEnumerable<Solid> solids) : base(solids)
        {

        }

    }
}
