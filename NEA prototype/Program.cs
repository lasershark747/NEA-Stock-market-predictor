using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; //for using a streamreader to read the response from the url
using System.Net; //for accessing the internet stuff (i think)
using Newtonsoft.Json; //for using the package
using static System.Net.WebRequestMethods;
using NEA_prototype;

//THis is a test
//This is a conformation that it works both ways

namespace trialWithStockMarketAPI
{
    class InfoAboutStock
    {
        string ticker { get; set; }
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

        public double AverageValue()
        {
            double sum = 0;
            foreach (prices pr in results)
            {
                sum += pr.GetVW();
            }
            return sum;
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

        public double GetVW()
        {
            return vw;
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            // Set up the response.
            //WebRequest request = WebRequest.Create(SetUpRequest());
            string url = "https://api.polygon.io/v2/aggs/ticker/AAPL/range/1/day/2022-01-01/2022-02-01?adjusted=true&sort=asc&limit=5000&apiKey=CM_QQuAvxVCV7hM8RS9jDCRIJh85Ux2v";
            WebRequest request = WebRequest.Create(url);
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();

            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();

            InfoAboutStock infoAboutStock = JsonConvert.DeserializeObject<InfoAboutStock>(responseFromServer);


            Console.WriteLine(responseFromServer);
            List<long> valueOfStock = new List<long>();
            foreach (prices pr in infoAboutStock.results)
            {
                valueOfStock.Add(pr.t);
            }

            //Line_Chart lineGraph = new Line_Chart(valueOfStock);
            //lineGraph.ShowDialog();



            foreach (prices pr in infoAboutStock.results)
            {
                Console.WriteLine(pr.t);
            }

            Console.WriteLine(valueOfStock.Count);
            Console.ReadKey();
        }

        public static string SetUpRequest()
        {
            string output = "https://api.polygon.io/v2/aggs/ticker/";
            Console.WriteLine("Please enter the ticker for the stock.");
            output += Console.ReadLine() + "/range/1/";
            Console.WriteLine("Please enter the time span for the request.\nThe only accepted time spans are: minute, hour, day, week, month, quater, year.");
            output += Console.ReadLine() + "/";
            Console.WriteLine("Please enter the start date for the analysis.\nPlease enter all dates in the for yyyy-mm-dd.");
            output += Console.ReadLine() + "/";
            Console.WriteLine("Please enter the end date for the analysis.\nPlease enter all dates in the for yyyy-mm-dd.");
            output += Console.ReadLine() + "?adjusted=true&sort=asc&limit=5000&apiKey=CM_QQuAvxVCV7hM8RS9jDCRIJh85Ux2v";
            Console.WriteLine(output);
            return output;
        }
    }
}