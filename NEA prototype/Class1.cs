using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NEA_prototype
{
    internal class SumOfResiduals
    {
        private List<(long, double)> data;
        private List<double> curve;


        public SumOfResiduals(List<(long, double)> data, List<double> curve)
        {
            this.data = data;
            this.curve = curve;
        }

        public BigFloat Residuals(int numOfdataPoints)
        {
            double sum = 0;
            int x = data.Count/numOfdataPoints;
            int count = 0;
            for (int i = 0; i < data.Count; i+=x)
            {
                double predicted = FOfX(data[i].Item1);
                sum += data[i].Item2 - predicted * predicted;
                Console.WriteLine(count);
                count++;
            }
            BigFloat variance = sum / (data.Count-curve.Count);
            Console.WriteLine(variance);
            Console.ReadKey();
            Console.Clear();
            return variance;
        }

        private double FOfX(long x)
        {
            double sum = 0; 
            for(int i = 0; i < curve.Count; i++)
            {
                sum += curve[i] * Math.Pow(x, i);
            }
            return sum;
        }
    }
}
