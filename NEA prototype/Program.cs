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
        //t is the time stamp for the start of the window in UNIX time --> milliseconds 
        //v is the trading volume in the time period
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
            int numOfStocks = 0;
            List<string> stockNames = new List<string>();
            List<List<(long, Double)>> stockPrices = new List<List<(long, Double)>>();
            while (true)
            {
                try
                {
                    Console.WriteLine("How many stocks would you like to graph?");
                    numOfStocks = int.Parse(Console.ReadLine());
                    if (numOfStocks < 1)
                    {
                        throw new FormatException();
                    }
                    break;
                }
                catch (System.FormatException)
                {
                    Console.WriteLine("Please enter a response in the correct format --> positive int");
                }
            }
            for (int i = 0; i < numOfStocks; i++)
            {
                int choice = LoadMenu();
                if (choice == 1)
                {
                    InfoAboutStock infoAboutStock1 = DoAPIRequest(1);
                    prices[] valuesOfStock1 = infoAboutStock1.GetPrices();
                    List<(long, Double)> points1 = new List<(long, Double)>();
                    foreach (prices pr in valuesOfStock1)
                    {
                        points1.Add((pr.GetTime(), pr.GetAveragePrice()));
                    }
                    GraphOfStockValue.AddNewSeries(points1, infoAboutStock1.ticker);
                    stockNames.Add(infoAboutStock1.ticker);
                    stockPrices.Add(points1);
                }
                else if (choice == 3)
                {
                    InfoAboutStock infoAboutStock1 = DoAPIRequest(2);
                    prices[] valuesOfStock1 = infoAboutStock1.GetPrices();
                    List<(long, Double)> points1 = new List<(long, Double)>();
                    foreach (prices pr in valuesOfStock1)
                    {
                        points1.Add((pr.GetTime(), pr.GetAveragePrice()));
                    }
                    GraphOfStockValue.AddNewSeries(points1, infoAboutStock1.ticker);
                    stockNames.Add(infoAboutStock1.ticker);
                    stockPrices.Add(points1);
                }
                else
                {
                    Console.WriteLine("PLEASE READ THE MENU");
                }
            }
            int howToDisplay;
            while (true)
            {
                try
                {
                    Console.WriteLine("How would you like to get the data?\nJust graph - 1\nJust table - 2\nBoth - 3");
                    howToDisplay = int.Parse(Console.ReadLine());
                    break;
                }
                catch (System.FormatException)
                {
                    Console.Clear();
                    Console.WriteLine("Please enter a response in the correct format --> 1 or 2 or 3");
                }

            }

            if (howToDisplay == 1)
            {
                GraphOfStockValue.ShowDialog();
            }
            else if (howToDisplay == 2)
            {
                long unixTime = ConvertToUNIXMilli();
                Console.WriteLine(unixTime);
                Console.WriteLine(binarySearch(unixTime, stockPrices[0]));
            }
            else
            {
                (long, double) point;
                while (true)
                {
                    long unixTime = ConvertToUNIXMilli();
                    Console.WriteLine(unixTime);
                    point = binarySearch(unixTime, stockPrices[0]);
                    Console.WriteLine(point);
                    Console.ReadKey();
                    //NEED to design table and then format it correctly and allow for the user to add values to it or change which values are shown
                    break;
                }
                
                List<(long,Double)> temp = new List<(long,Double)> ();
                temp.Add(point);
                GraphOfStockValue.AddNewSeries(temp, "clostest");
                


                GraphOfStockValue.ShowDialog();
            }
            Console.WriteLine("Goodbye");
            Console.ReadKey();
        }
        public static int LoadMenu()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Would you like to do a new analysis (1) or use an old analysis (2) (Doesn't work atm)");
                    int response = int.Parse(Console.ReadLine());
                    if (response != 1 && response != 2 && response != 3)
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

        public static InfoAboutStock DoAPIRequest(int choice)
        {
            //DO ERROR HANDLING HERE
            InfoAboutStock infoAboutStock;
            while (true)
            {
                try
                {
                    WebRequest request;
                    if (choice == 1)
                    {
                        request = WebRequest.Create(SetUpRequest());
                    }
                    else
                    {
                        string exampleAddress = "https://api.polygon.io/v2/aggs/ticker/NVDA/range/1/day/2022-01-01/2023-09-01?adjusted=true&sort=asc&limit=5000&apiKey=CM_QQuAvxVCV7hM8RS9jDCRIJh85Ux2v";
                        request = WebRequest.Create(exampleAddress);
                    }

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    Stream dataStream = response.GetResponseStream();

                    StreamReader reader = new StreamReader(dataStream);

                    string responseFromServer = reader.ReadToEnd();

                    infoAboutStock = JsonConvert.DeserializeObject<InfoAboutStock>(responseFromServer);

                    break;
                }

                catch (System.Net.WebException)
                {
                    //add any common errors for the API request in here 
                    Console.Clear();
                    Console.WriteLine("Error in the API request\nSome possible errors are:\nEntered data incorrectly\nStock isn't on the NASDAQ\nDate inputed isn't within correct margin");
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
                Console.WriteLine("The API request is: " + output);
                Console.WriteLine("Is this correct?\ny or n");
                string response = Console.ReadLine();
                if (response == "y")
                    break;
                else if(response == "n")
                {
                    Console.WriteLine("Remaking API url");
                    System.Threading.Thread.Sleep(1000);
                    Console.Clear();
                }
            }
            return output;
        }

        public static long ConvertToUNIXMilli()
        {
            long unix = 0;
            string[] seperated;
            while (true)
            {
                try
                {
                    Console.WriteLine("Please enter the date in the form yyyy-mm-dd");
                    string yyyymmdd = Console.ReadLine();
                    seperated = yyyymmdd.Split('-');
                    if (seperated.Length != 3)
                    {
                        throw new FormatException();
                    }
                    break;
                }
                catch (System.FormatException)
                {
                    Console.WriteLine("Please enter a response in the correct format --> yyyy-mm-dd");
                }
            }

            for (int i = 1971; i <= int.Parse(seperated[0]); i++)
            {
                if (i % 4 == 0) unix += 86400 * 366;

                else unix += 86400 * 365;
            }
            unix += (long.Parse(seperated[2]) - 1) * 86400;
            long[] daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            for (int i = 0; i < int.Parse(seperated[1]) - 1; i++)
            {
                unix += daysInMonth[i] * 86400;
            }

            return (unix+4*60*60)*1000;
        }

        public static (long, double) binarySearch(long unixTime, List<(long,double)> stockValues)
        {
            Random r = new Random();
            long number =unixTime;
            List<(long, double)> trialList = stockValues;
            int min = 0;
            int max = trialList.Count - 1;
            while (true)
            {
                int midPoint = (min + max) / 2;
                if (number == trialList[midPoint].Item1)
                {
                    return trialList[midPoint];
                }
                else if(min >= max)
                {
                    return trialList[max];
                }
                else if (number > trialList[midPoint].Item1)
                {
                    min = midPoint+1;
                }
                else if(number < trialList[midPoint].Item1)
                {
                    max = midPoint-1;
                }
                else Console.WriteLine("error");

            }

        }
    }
} 