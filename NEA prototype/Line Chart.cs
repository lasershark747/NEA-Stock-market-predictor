using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NEA_prototype
{
    public partial class Line_Chart : Form
    {
        List<double> points = new List<double>();

        public Line_Chart(List<double> points)
        {
            InitializeComponent();
            this.points = points;


        }

        private void Line_Chart_Load(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
