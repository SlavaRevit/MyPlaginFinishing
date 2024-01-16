using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectionFilter.Collections
{
	public class Coordinates : List<double>
	{
        public Coordinates(IEnumerable<double> coordinates) : base (coordinates) 
        {
            
        }
    }
}
