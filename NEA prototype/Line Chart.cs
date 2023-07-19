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

        public Line_Chart(List<(long,Double)> values)
        {
            InitializeComponent();
            this.points = values;
        }

        private void Line_Chart_Load(object sender, EventArgs e)
        {
            chart1.Series.Add("data");
            chart1.Series["data"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            foreach ((long,double) point in points) 
            {
                Console.WriteLine(point.Item2);
                chart1.Series["data"].Points.AddXY(point.Item1,point.Item2);
            }
            
        }
    }
}
