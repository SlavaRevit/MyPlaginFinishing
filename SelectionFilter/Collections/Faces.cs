using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectionFilter.Collections
{
	public class Faces : List<Face>
	{
        public Faces()
        {
            
        }

        public Faces(IEnumerable<Face> faces) : base(faces)
        {

        }

    }
}
