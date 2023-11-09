using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace NEA_prototype
{
    public partial class Line_Chart : Form
    {
        List<string> names = new List<string>();
        string[] colours;
        private Random r = new Random();
        private long endDate;

        public Line_Chart()
        {
            InitializeComponent();
            GenerateColours();
        }

        private void GenerateColours()
        {
            string colourList = "LightSalmon.LightSeaGreen.Aqua.LightSkyBlue.Aquamarine.LightSlateGray.LightSteelBlue.Lime.Black.LimeGreen.Blue.Magenta.BlueViolet.Maroon.Brown.MediumAquamarine.MediumBlue.CadetBlue.MediumOrchid.Chartreuse.MediumPurple.Chocolate.MediumSeaGreen.Coral.MediumSlateBlue.CornflowerBlue.MediumSpringGreen.MediumTurquoise.Crimson.MediumVioletRed.Cyan.MidnightBlue.DarkBlue.DarkCyan.DarkGoldenrod.DarkGreen.Navy.DarkMagena.Olive.DarkOliveGreen.OliveDrab.Orange.DarkOrchid.OrangeRed.DarkRed.Orchid.PaleGoldenrod.DarkSeaGreen.PaleGreen.DarkSlateBlue.PaleTurquoise.DarkSlateGray.PaleVioletRed.DarkTurquoise.DarkViolet.DeepPink.Peru.DeepSkyBlue.DimGray.Plum.DodgerBlue.PowderBlue.Firebrick.Purple.Red.ForestGreen.RosyBrown.Fuschia.RoyalBlue.SaddleBrown.Salmon.Gold.SandyBrown.Goldenrod.SeaGreen.Gray.Green.Sienna.GreenYellow.Silver.SkyBlue.HotPink.SlateBlue.IndianRed.SlateGray.Indigo.SpringGreen.Khaki.SteelBlue.Tan.Teal.LawnGreen.Tomato.Turquoise.LightCoral.Violet.LightGreen.LightPink.YellowGreen";
            colours = colourList.Split('.');
        }

        private void Line_Chart_Load(object sender, EventArgs e) { }

        public void AddNewSeries(List<(long, double)> points, string name)
        {
            bool duplicate = false;
            string colour = colours[r.Next(0, colours.Length)];

            foreach (string name2 in names)
            {
                while (true)
                {
                    if (name == name2)
                    {
                        name = name + "2";
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (!duplicate)
            {
                names.Add(name);

                chart1.Series.Add(name);
                chart1.Series[name].ChartType = SeriesChartType.Spline;
                chart1.Series[name].Color = Color.FromName(colour);

                foreach ((long, double) point in points)
                {
                    chart1.Series[name].Points.AddXY(point.Item1, point.Item2);
                }

                chart1.Series.Add(name + "1");
                chart1.Series[name + "1"].ChartType = SeriesChartType.Point;
                chart1.Series[name + "1"].Color = Color.FromName(colour);

                foreach ((long, double) point in points)
                {
                    chart1.Series[name + "1"].Points.AddXY(point.Item1, point.Item2);
                }
            }
        }
        public void AddRegressionCurve(List<double> equation, long startDate, string nameOfStock)
        {
            bool duplicate = false;

            string colour = colours[r.Next(0, colours.Length)];
            string name = nameOfStock + " prediction";

            foreach (string name2 in names)
            {
                while (true)
                {
                    if (name == name2)
                    {
                        name = name + "2";
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (!duplicate)
            {
                names.Add(name);
                chart1.Series.Add(name);
                chart1.Series[name].ChartType = SeriesChartType.Line;
                chart1.Series[name].Color = Color.FromName(colour);
                
                if (endDate == 0)
                {
                    Console.WriteLine("Now choosing date that you want to see the prediction up to.");
                    endDate = Program.ConvertToUNIXMilli();
                }

                for (long i = startDate; i < endDate; i += 86400)
                {
                    double predictiedValue = 0;

                    for (int j = 0; j < equation.Count; j++)
                    {
                        predictiedValue += (double)(equation[j] * Math.Pow(i, j));
                    }

                    if (predictiedValue < 0)
                    {
                        break;
                    }
                    else
                    {
                        chart1.Series[name].Points.AddXY(i, predictiedValue);
                    }
                }
            }
        }
    }
}
