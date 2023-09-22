using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace NEA_prototype
{
    public partial class Line_Chart : Form
    {
        List<string> names = new List<string>();
        string[] colours; 
        private Random r = new Random();

        public Line_Chart()
        {
            InitializeComponent();
            GenerateColours();
        }

        private void GenerateColours()
        {
            string colourList = "AliceBlue.LightSalmon.AntiqueWhite.LightSeaGreen.Aqua.LightSkyBlue.Aquamarine.LightSlateGray.Azure.LightSteelBlue.Beige.LightYellow.Bisque.Lime.Black.LimeGreen.BlanchedAlmond.Linen.Blue.Magenta.BlueViolet.Maroon.Brown.MediumAquamarine.BurlyWood.MediumBlue.CadetBlue.MediumOrchid.Chartreuse.MediumPurple.Chocolate.MediumSeaGreen.Coral.MediumSlateBlue.CornflowerBlue.MediumSpringGreen.Cornsilk.MediumTurquoise.Crimson.MediumVioletRed.Cyan.MidnightBlue.DarkBlue.MintCream.DarkCyan.MistyRose.DarkGoldenrod.Moccasin.DarkGray.NavajoWhit.DarkGreen.Navy.DarkKhaki.OldLace.DarkMagena.Olive.DarkOliveGreen.OliveDrab.DarkOrange.Orange.DarkOrchid.OrangeRed.DarkRed.Orchid.DarkSalmon.PaleGoldenrod.DarkSeaGreen.PaleGreen.DarkSlateBlue.PaleTurquoise.DarkSlateGray.PaleVioletRed.DarkTurquoise.PapayaWhip.DarkViolet.PeachPuff.DeepPink.Peru.DeepSkyBlue.Pink.DimGray.Plum.DodgerBlue.PowderBlue.Firebrick.Purple.FloralWhite.Red.ForestGreen.RosyBrown.Fuschia.RoyalBlue.Gainsboro.SaddleBrown.GhostWhite.Salmon.Gold.SandyBrown.Goldenrod.SeaGreen.Gray.Seashell.Green.Sienna.GreenYellow.Silver.Honeydew.SkyBlue.HotPink.SlateBlue.IndianRed.SlateGray.Indigo.Snow.Ivory.SpringGreen.Khaki.SteelBlue.Lavender.Tan.LavenderBlush\r\n.Teal.LawnGreen.Thistle.LemonChiffon.Tomato.LightBlue.Turquoise.LightCoral.Violet.LightCyan.Wheat.LightGoldenrodYellow.White.LightGreen.WhiteSmoke.LightGray.Yellow.LightPink.YellowGreen";
            colours = colourList.Split('.');
        }

        private void Line_Chart_Load(object sender, EventArgs e)
        {

        }
        public void AddNewSeries(List<(long, Double)> points, string name)
        {
            bool duplicate = false;
            string colour = colours[r.Next(0, colours.Length)];

            foreach (string name2 in names)
            {
                if (name == name2)
                {
                    Console.WriteLine("A series with the same name already exists.");
                    Console.WriteLine("Would you like to still display the graph? \ny or n");
                    if (Console.ReadLine() == "n")
                    {
                        duplicate = true;
                    }
                    else
                    {
                        name = name + "2";
                    }
                    break;
                }

            }
            if (!duplicate)
            {
                names.Add(name);
                chart1.Series.Add(name);
                chart1.Series[name].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                chart1.Series[name].Color = System.Drawing.Color.FromName(colour);
                foreach ((long, double) point in points)
                {
                    chart1.Series[name].Points.AddXY(point.Item1, point.Item2);
                }
                chart1.Series.Add(name + "1");
                chart1.Series[name + "1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                chart1.Series[name + "1"].Color = System.Drawing.Color.FromName(colour);
                foreach ((long, double) point in points)
                {
                    chart1.Series[name + "1"].Points.AddXY(point.Item1, point.Item2);
                }
            }
        }
    }
}
