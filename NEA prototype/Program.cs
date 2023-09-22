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
using System.Windows.Forms;
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
            Console.WriteLine("Welcome to SPAM");
            Line_Chart GraphOfStockValue = new Line_Chart();
            int numOfStocks =0;
            while (true)
            {
                try
                {
                    Console.WriteLine("How many stocks would you like to graph?");
                    numOfStocks = int.Parse(Console.ReadLine());
                    if(numOfStocks < 1)
                    {
                        throw new FormatException();
                    }
                }
                catch(System.FormatException)
                {
                    Console.WriteLine("Please enter a response in the correct format --> positive int");
                }

                break;
            }
            for (int i = 0; i < numOfStocks; i++)
            {
                if (LoadMenu() == 1)
                {
                    InfoAboutStock infoAboutStock1 = DoAPIRequest();
                    prices[] valuesOfStock1 = infoAboutStock1.GetPrices();
                    List<(long, Double)> points1 = new List<(long, Double)>();
                    foreach (prices pr in valuesOfStock1)
                    {
                        points1.Add((pr.GetTime(), pr.GetAveragePrice()));
                    }
                    GraphOfStockValue.AddNewSeries(points1, infoAboutStock1.ticker);
                }
                else
                {
                    Console.WriteLine("PLEASE READ THE MENU");
                }
            }
            GraphOfStockValue.ShowDialog();
            Console.WriteLine("Goodbye");
            Console.ReadKey();
        }

        private static int LoadMenu()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Would you like to do a new analysis (1) or use an old analysis (2) (Doesn't work atm)");
                    int response = int.Parse(Console.ReadLine());
                    if (response != 1 && response != 2)
                    {
                        throw new FormatException();
                    }
                    return response;
                }
                catch (System.FormatException)
                {
                    Console.Clear();
                    Console.WriteLine("Please enter a response in the correct format --> 1 or 2");
                }
            }
        }

        public static InfoAboutStock DoAPIRequest()
        {
            //DO ERROR HANDLING HERE
            InfoAboutStock infoAboutStock;
            while (true)
            {
                try
                {
                    string exampleAddress = "https://api.polygon.io/v2/aggs/ticker/AAPL/range/1/day/2022-01-01/2022-02-01?adjusted=true&sort=asc&limit=5000&apiKey=CM_QQuAvxVCV7hM8RS9jDCRIJh85Ux2v";
                    //WebRequest request = WebRequest.Create(exampleAddress);    
                    WebRequest request = WebRequest.Create(SetUpRequest());
                    
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    Stream dataStream = response.GetResponseStream();

                    StreamReader reader = new StreamReader(dataStream);

                    string responseFromServer = reader.ReadToEnd();

                    infoAboutStock = JsonConvert.DeserializeObject<InfoAboutStock>(responseFromServer);

                    break;
                }
                
                catch(System.Net.WebException)
                {
                    Console.WriteLine("Error in the API reuest please double check that you are entering data in the correct form");
                }
                
            }
            return infoAboutStock;
        }


        public static string SetUpRequest()
        {
            //ADD A WAY TO REMAKE THE API REQUEST IF THE USER ENTERS INFO INCORRECTLY --> WON'T BE DETECTABLE HERE
            string output;
            while (true)
            {
                output = "https://api.polygon.io/v2/aggs/ticker/";
                Console.WriteLine("Please enter the ticker for the stock on the NASDAQ.");
                output += Console.ReadLine() + "/range/1/";

                Console.WriteLine("Please enter the time span for the request.\nThe only accepted time spans are: minute, hour, day, week, month, quater, year.");
                output += Console.ReadLine() + "/";

                Console.WriteLine("Please enter the start date for the analysis.\nPlease enter all dates in the form yyyy-mm-dd.");
                output += Console.ReadLine() + "/";

                Console.WriteLine("Please enter the end date for the analysis.\nPlease enter all dates in the form yyyy-mm-dd.");
                output += Console.ReadLine() + "?adjusted=true&sort=asc&limit=5000&apiKey=CM_QQuAvxVCV7hM8RS9jDCRIJh85Ux2v";
                Console.Clear();
                Console.WriteLine("The API request is: " + output);
                Console.WriteLine("Is this correct?\ny or n");
                if (Console.ReadLine() != "y")
                {
                    Console.WriteLine("Remaking API url");
                    System.Threading.Thread.Sleep(1000);
                    Console.Clear();
                }
                else break;
            }
            return output;
        }
    }
} 