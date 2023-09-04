using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; 
using System.Net; 
using Newtonsoft.Json; 
using static System.Net.WebRequestMethods;
using NEA_prototype;
/*
curve needs to be in form x^0 --> x^n rather then x^n --> x^0



*/
namespace trialWithStockMarketAPI
{
    class InfoAboutStock
    {
        public string ticker { get; set; }
        int queryCount { get; set; }
        int resultsCount { get; set; }
        bool adjusted { get; set; }
        public prices[] results { get; set; }
        string status { get; set; }
        string request_id { get; set; }
        int count { get; set; }


        public string GetTicker()
        {
            return ticker;
        }
        public prices[] GetPrices()
        {
            return results;
        }
    }


    class prices
    {
        double v { get; set; }
        public double vw { get; set; }
        double o { get; set; }
        double c { get; set; }
        double h { get; set; }
        double l { get; set; }
        public long t { get; set; }
        int n { get; set; }

        //c is the close price
        //h is the highest price in the time period
        //l is the lowest price in the time period
        //n is the number of transactions in the time period
        //o is the open price
        //t is the time stamp for the start of the window
        // is the trading volume in the time period
        //vw is the wolume weight average price (this is the main peice of data that i'll be using)

        public double GetAveragePrice()
        {
            return vw;
        }
        public long GetTime()
        {
            return t;
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            string exampleAddress = "https://api.polygon.io/v2/aggs/ticker/AAPL/range/1/day/2022-01-01/2022-02-01?adjusted=true&sort=asc&limit=5000&apiKey=CM_QQuAvxVCV7hM8RS9jDCRIJh85Ux2v";
            WebRequest request = WebRequest.Create(SetUpRequest());

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();

            InfoAboutStock infoAboutStock = JsonConvert.DeserializeObject<InfoAboutStock>(responseFromServer);           
            
            prices[] valuesOfStock = infoAboutStock.results;
            List<(long,Double)> points = new List<(long,Double)> ();
            foreach(prices pr in valuesOfStock)
            {
                points.Add((pr.t,pr.GetAveragePrice()));
            }

            Line_Chart GraphOfStockValue = new Line_Chart(points,infoAboutStock.ticker);
            foreach((long, Double) pr in points)
            {
                Console.WriteLine(pr.Item1);
            }
            foreach ((long, Double) pr in points)
            {
                Console.WriteLine(pr.Item2);
            }
            GraphOfStockValue.ShowDialog();
            
            Console.ReadKey();
        }

        public static string SetUpRequest()
        {
            string output = "https://api.polygon.io/v2/aggs/ticker/";
            Console.WriteLine("Please enter the ticker for the stock on the NASDAQ.");
            output += Console.ReadLine() + "/range/1/";

            Console.WriteLine("Please enter the time span for the request.\nThe only accepted time spans are: minute, hour, day, week, month, quater, year.");
            output += Console.ReadLine() + "/";

            Console.WriteLine("Please enter the start date for the analysis.\nPlease enter all dates in the form yyyy-mm-dd.");
            output += Console.ReadLine() + "/";

            Console.WriteLine("Please enter the end date for the analysis.\nPlease enter all dates in the form yyyy-mm-dd.");
            output += Console.ReadLine() + "?adjusted=true&sort=asc&limit=5000&apiKey=CM_QQuAvxVCV7hM8RS9jDCRIJh85Ux2v";
            Console.Clear();
            Console.WriteLine("The address for the API request is:\n" + output);
            return output;
        }
    }
}