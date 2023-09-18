using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA_prototype
{
    internal class SumOfResiduals
    {
        private List<(long, Double)> data;
        public List<decimal> curve;


        public SumOfResiduals(List<(long, double)> data, List<decimal> curve)
        {
            this.data = data;
            this.curve = curve;
        }


        public decimal DoSumOfResiduals()
        {
            decimal RSS = 0;
            decimal TV = 0;

            RSS = ResidualSumOfSquares();
            TV = TotalVariance();

            return 1 - RSS/TV;
        }


        private decimal ResidualSumOfSquares()
        {
            decimal sum = 0;
            foreach ((long, Double) p in data)
            {
                sum += ((decimal)p.Item2 - FOfX(p.Item1)) * ((decimal)p.Item2 - FOfX(p.Item1));
            }

            return sum;
        }

        private decimal TotalVariance()
        {
            decimal sum = 0;

            decimal mean = FindTheMean();

            foreach ((long, Double) p in data)
            {
                sum += (mean - (decimal)p.Item2) * (mean - (decimal)p.Item2);
            }

            return sum;
        }


        private decimal FOfX(long x)
        {
            decimal sum = 0; 
            for(int i = 0; i < curve.Count; i++)
            {
                sum += curve[i] * (decimal)Math.Pow(x, i);
            }
            return sum;
        }

        private decimal FindTheMean()
        {
            decimal count = 0;
            decimal sum = 0;
            foreach((long, Double) p in data)
            {
                sum += (decimal)p.Item2;
                count++;
            }

            return sum / count;
        }
    }
}
