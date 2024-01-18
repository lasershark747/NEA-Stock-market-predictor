using System;
using System.Collections.Generic;

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
        public double Residuals()
        {
            double sum = 0;
            
            for (int i = 0; i < data.Count; i++)
            {
                double predicted = FOfX(data[i].Item1);
                sum += Math.Pow(data[i].Item2 - predicted,2);
            }

            double variance = sum / (data.Count-curve.Count);
            return variance;
        }
        private double FOfX(long x)
        {
            double sum = 0; 

            for(int i = 0; i < curve.Count; i++)
            {
                sum += curve[i] * double.Parse(Math.Pow(x, i).ToString());
            }

            return sum;
        }
    }
}