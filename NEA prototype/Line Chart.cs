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
        List<(long,Double)> points;
        string name;

        public Line_Chart(List<(long,Double)> values, string name)
        {
            InitializeComponent();
            this.points = values;
            this.name = name;
        }

        private void Line_Chart_Load(object sender, EventArgs e)
        {
            chart1.Series.Add(name);
            chart1.Series[name].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            foreach ((long,double) point in points) 
            {
                chart1.Series[name].Points.AddXY(point.Item1,point.Item2);
            }
           
        }
    }
}
