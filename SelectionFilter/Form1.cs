using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SelectionFilter
{
	public partial class Form1 : Form
	{
        public string Base { get; set; }
		public string Height { get; set; }
		public string Offset { get; set; }

		public Form1()
		{
			InitializeComponent();
			
		}




		private void OkButton_Click_1(object sender, EventArgs e)
		{
			Base = BaseBox.Text;
			Height = HeightBox.Text;
			Offset = offsetBox.Text;

			OkButton.DialogResult = DialogResult.OK;
			Close();
			return;
		}
	}
}
