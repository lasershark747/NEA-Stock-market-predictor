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
        private List<(long, BigFloat)> data;
        public List<BigFloat> curve;


        public SumOfResiduals(List<(long, BigFloat)> data, List<BigFloat> curve)
        {
            this.data = data;
            this.curve = curve;
        }


        public double DoSumOfResiduals()
        {
            decimal RSS = 0;
            decimal TV = 0;

            RSS = ResidualSumOfSquares();
            TV = TotalVariance();

            return double.Parse((1 - RSS / TV).ToString()); ;
        }


        private decimal ResidualSumOfSquares()
        {
            decimal sum = 0;
            foreach ((long, BigFloat) p in data)
            {
                sum += ((decimal)p.Item2 - FOfX(p.Item1)) * ((decimal)p.Item2 - FOfX(p.Item1));
            }

            return sum;
        }

        private decimal TotalVariance()
        {
            decimal sum = 0;

            decimal mean = FindTheMean();

            foreach ((long, BigFloat) p in data)
            {
                sum += (mean - (decimal)double.Parse(p.Item2.ToString())) * (mean - (decimal)double.Parse(p.Item2.ToString()));
            }

            return sum;
        }


        private decimal FOfX(long x)
        {
            decimal sum = 0; 
            for(int i = 0; i < curve.Count; i++)
            {
                sum += decimal.Parse(curve[i].ToString()) * (decimal)Math.Pow(x, i);
            }
            return sum;
        }

        private decimal FindTheMean() => data.Sum(p => (decimal) p.Item2) / data.Count;
    }
}
