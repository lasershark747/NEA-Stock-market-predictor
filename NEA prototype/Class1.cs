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
            BigFloat RSS = 0;
            BigFloat TV = 0;

            RSS = ResidualSumOfSquares();
            TV = TotalVariance();
            Console.WriteLine(data.Count);
            return double.Parse((1 - RSS / TV).ToString()); ;
        }


        private BigFloat ResidualSumOfSquares()
        {
            BigFloat sum = 0;
            foreach ((long, BigFloat) p in data)
            {
                sum += (p.Item2 - FOfX(p.Item1)) * (p.Item2 - FOfX(p.Item1));
            }

            return sum;
        }

        private BigFloat TotalVariance()
        {
            BigFloat sum = 0;

            BigFloat mean = FindTheMean();

            foreach ((long, BigFloat) p in data)
            {
                sum += (mean - p.Item2) * (mean -p.Item2);
            }

            return sum;
        }


        private BigFloat FOfX(long x)
        {
            BigFloat sum = 0; 
            for(int i = 0; i < curve.Count; i++)
            {
                sum += curve[i] * (BigFloat)Math.Pow(x, i);
            }
            return sum;
        }

        private BigFloat FindTheMean()
        {
            BigFloat count = data.Count;
            BigFloat sum = 0;
            foreach((long,BigFloat) p in data)
            {
                sum += p.Item2;
            }
            return sum/count;

        }
    }
}
