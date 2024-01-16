using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectionFilter.Models
{
	public class BoxDimension
	{
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }


        public BoxDimension(double length, double width, double height)
        {
            Length = length;
            Width = width;  
            Height = height;    
        }


    }

   

}
