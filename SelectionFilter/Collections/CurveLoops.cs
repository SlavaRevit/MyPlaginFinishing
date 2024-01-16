using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectionFilter.Collections
{
	public class CurveLoops : List<CurveLoop>
	{

        public CurveLoops()
        {
              
        }

        public CurveLoops(IEnumerable<CurveLoop> curveLoops) : base(curveLoops)
        {
                
        }

    }
}
