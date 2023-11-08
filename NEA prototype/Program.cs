using System;
using System.Collections.Generic;
using System.IO; 
using System.Net; 
using Newtonsoft.Json; 
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace NEA_prototype
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
        public prices[] GetResults()
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

        //t is the time stamp for the start of the window in UNIXMilli 
        //vw is the wolume weight average price

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
            Line_Chart GraphOfStockValue = new Line_Chart();
            PolynomialRegression p = new PolynomialRegression();

            List<string> stockNames = new List<string>();
            List<List<(long, double)>> stockPrices = new List<List<(long, double)>>();
            bool exitLoop = false;

            int numOfStocks = 0;

            Console.WriteLine("Welcome to SPAM");    
            
            while (!exitLoop)
            {
                try
                {
                    Console.WriteLine("How many stocks would you like to graph?");

                    numOfStocks = int.Parse(Console.ReadLine());

                    if (numOfStocks < 1|| numOfStocks > 8)
                    {
                        throw new FormatException();
                    }

                    exitLoop = true;
                }
                catch (System.FormatException)
                {
                    Console.WriteLine("Please enter a response in the correct format --> 1-8");
                }
                catch(System.OverflowException)
                {
                    Console.WriteLine("Please enter a response in the correct format --> 1-8");
                 }
            }
            exitLoop = false;
            for (int i = 0; i < numOfStocks; i++)
            {
                int choice = LoadMenu();

                if (choice == 1)
                {
                    InfoAboutStock infoAboutStock1 = DoAPIRequest(1);
                    prices[] valuesOfStock1 = infoAboutStock1.results;
                    List<(long, double)> points1 = new List<(long, double)>();

                    foreach (prices pr in valuesOfStock1)
                    {
                        points1.Add((pr.t/1000, pr.vw));
                    }
                    stockNames.Add(infoAboutStock1.ticker);
                    stockPrices.Add(points1);
                    }
                else if (choice == 2)
                {
                    InfoAboutStock infoAboutStock1 = DoAPIRequest(2);
                    prices[] valuesOfStock1 = infoAboutStock1.results;
                    List<(long, double)> points1 = new List<(long, double)>();
                    foreach (prices pr in valuesOfStock1)
                    {
                        points1.Add((pr.GetTime()/1000, pr.GetAveragePrice()));
                    }
                    stockNames.Add(infoAboutStock1.ticker);
                    stockPrices.Add(points1);
                }
                else
                {
                    Console.WriteLine("PLEASE READ THE MENU");
                }
            }

            int howToDisplay = 0;

            while (!exitLoop)
            {
                try
                {
                    Console.WriteLine("How would you like to get the data?\nJust graph - 1\nJust table - 2\nBoth - 3");

                    howToDisplay = int.Parse(Console.ReadLine());
                    if(howToDisplay > 3 || howToDisplay < 1) 
                    {
                        throw new FormatException();
                    }

                    exitLoop = true;
                }
                catch (System.FormatException)
                {
                    Console.Clear();
                    Console.WriteLine("Please enter a response in the correct format --> 1 or 2 or 3");
                }

            }

            if (howToDisplay == 1)
            {
                for (int i = 0; i < stockNames.Count; i++)
                {
                    GraphOfStockValue.AddNewSeries(stockPrices[i], stockNames[i]);
                    List<double> coeffcients = p.DoPolynomialRegression(stockPrices[i]);
                    GraphOfStockValue.AddRegressionCurve(coeffcients, 1641168000, stockNames[i]);
                }

                GraphOfStockValue.ShowDialog();
            }
            else if (howToDisplay == 2)
            {
                List<List<double>> listOfCurves = new List<List<double>>();
                for (int i = 0; i < stockNames.Count; i++)
                {
                    listOfCurves.Add(p.DoPolynomialRegression(stockPrices[i]));
                }

                DisplayTable(stockPrices, stockNames,listOfCurves);
            }
            else
            {
                List<List<double>> listOfCurves = new List<List<double>>();


                for (int i = 0; i < stockNames.Count; i++)
                {
                    GraphOfStockValue.AddNewSeries(stockPrices[i], stockNames[i]);
                    List<double> coeffcients = p.DoPolynomialRegression(stockPrices[i]);
                    GraphOfStockValue.AddRegressionCurve(coeffcients, 1641168000, stockNames[i]);
                    listOfCurves.Add(coeffcients);
                }
                DisplayTable(stockPrices, stockNames, listOfCurves);
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
                    Console.WriteLine("Would you like to do a new analysis (1) or use example analysis (2)");

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

        public static InfoAboutStock DoAPIRequest(int choice)
        {
            InfoAboutStock infoAboutStock = new InfoAboutStock();
            bool exitLoop = false;
            while (!exitLoop)
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

                    exitLoop = true;
                }

                catch (System.Net.WebException)
                {
                    Console.Clear();
                    if (choice == 2)
                    {
                        Console.WriteLine("Please check you internet connection");
                    }
                    else
                    {
                        Console.WriteLine("Error in the API request\nSome possible errors are:\nEntered data incorrectly\nStock isn't on the NASDAQ\nDate inputed isn't within correct time span --> up to 2 year in the past\nInternet may be down");
                    }
                }
            }
            return infoAboutStock;
        }
        public static string SetUpRequest()
        {
            string output = "";
            bool exitMainLoop = false;

            while (!exitMainLoop)
            {
                output = "https://api.polygon.io/v2/aggs/ticker/";
                string regExDate = "202[1-3]\\-(0[1-9])|(1[0-2])\\-([0-2]\\d)|(3[01])";
                string regExTicker = "\\w+";
                bool exitLoop = false;
                while (!exitLoop)
                {
                    Console.WriteLine("Please enter the ticker for the stock on the NASDAQ.");
                    string buffer = Console.ReadLine();
                    if (Regex.IsMatch(buffer, regExTicker))
                    {
                        output += buffer + "/range/1/";
                        exitLoop = true;
                    }
                    else
                    {
                        Console.WriteLine("ticker in incorrect format");
                    }
                }
                exitLoop = false;
                while (!exitLoop)
                {
                    Console.WriteLine("Please enter the time span for the request.\nThe only accepted time spans are: minute, hour, day, week, month, quater, year.");
                    string buffer = Console.ReadLine();
                    if (buffer == "minute"|| buffer == "hour" || buffer == "day" || buffer == "week" || buffer == "month" || buffer == "quater" || buffer == "year")
                    {
                        output += buffer + "/";
                        exitLoop = true;
                    }
                    else
                    {
                        Console.WriteLine("time span in incorrect format");
                    }
                }
                exitLoop = false;
                while (!exitLoop)
                {
                    Console.WriteLine("Please enter the start date for the analysis.\nPlease enter all dates in the form yyyy-mm-dd.");
                    string buffer = Console.ReadLine();

                    if (Regex.IsMatch(buffer, regExDate))
                    {
                        output += buffer + "/";
                        exitLoop = true;
                    }
                    else
                    {
                        Console.WriteLine("date in incorrect format or is out of range");
                    }
                }
                exitLoop = false;
                while (!exitLoop)
                {
                    Console.WriteLine("Please enter the end date for the analysis.\nPlease enter all dates in the form yyyy-mm-dd.");
                    string buffer = Console.ReadLine();
                    if (Regex.IsMatch(buffer, regExDate))
                    {
                        output += buffer;
                        exitLoop = true;
                    }
                    else
                    {
                        Console.WriteLine("date in incorrect format or is out of range");
                    }
                }
                exitLoop = false;

                output += "?adjusted=true&sort=asc&limit=5000&apiKey=CM_QQuAvxVCV7hM8RS9jDCRIJh85Ux2v";


                exitLoop = false;
                while (!exitLoop)
                {
                    Console.WriteLine("The API request is: " + output);


                    Console.WriteLine("Is this correct?\ny or n");

                    string response = Console.ReadLine();

                    if (response == "y")
                    {
                        exitMainLoop = true;
                        exitLoop = true;
                    }
                    else if (response == "n")
                    {
                        Console.WriteLine("Remaking API url");
                        System.Threading.Thread.Sleep(1000);
                        Console.Clear();
                        exitLoop = true;
                    }
                    else
                    {
                        Console.WriteLine("Please enter respoense in the coreect format --> lowercase y or n");
                    }
                }
            }
            return output;
        }

        public static long ConvertToUNIXMilli()
        {
            long unix = 0;
            string[] seperated = new string[3];
            bool exitLoop = false;
            string regExDate = "202[4-9]\\-(0[1-9])|(1[0-2])\\-([0-2]\\d)|(3[01])";


            while (!exitLoop)
            {
                Console.WriteLine("Please enter the date.\nPlease enter all dates in the form yyyy-mm-dd.");
                string buffer = Console.ReadLine();

                if (Regex.IsMatch(buffer, regExDate))
                {
                    seperated = buffer.Split('-');
                    exitLoop = true;
                }
                else
                {
                    Console.WriteLine("date in incorrect format or is out of range");
                }
            }

            for (int i = 1971; i <= int.Parse(seperated[0]); i++)
            {
                if (i % 4 == 0) unix += 86400 * 366;

                else unix += 86400 * 365;
            }


            long[] daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            for (int i = 0; i < int.Parse(seperated[1]) - 1; i++)
            {
                unix += daysInMonth[i] * 86400;
            }

            unix += (long.Parse(seperated[2]) - 1) * 86400;

            return (unix+4*60*60);
        }
        public static string ConvertToyyyymmdd(long unixTime)
        {
            unixTime -= 4 * 60 * 60;
            string date = "";
            int year = 1970;
            int month = 1;
            int day = 0;
            int count = 2;
            bool exitLoop = false;

            while (!exitLoop)
            {
                if (count % 4 == 0)
                {
                    unixTime -= 86400 * 366;
                    year++;
                    count++;
                }
                else
                {
                    unixTime -= 86400 * 365;
                    year++;
                    count++;
                }
                if (unixTime < 0)
                {
                    year--;
                    if (count % 4 == 1)
                    {
                        unixTime += 86400 * 366;
                    }
                    else
                    {
                        unixTime += 86400 * 365;
                    }

                    date += year + "-";

                    exitLoop = true;
                }
            }

            count = 0;
            exitLoop = false;

            long[] daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            while (!exitLoop)
            {
                if (unixTime - daysInMonth[count] * 86400 < 0)
                {
                    exitLoop = true;
                }
                else
                {
                    unixTime -= daysInMonth[count] * 86400;
                    count++;
                    month++;
                }
            }

            if (month >= 10)
            {
                date += month + "-";
            }
            else
            {
                date += "0" + month + "-";
            }

            while (unixTime > 0)
            {
                unixTime -= 24 * 3600;
                day++;
            }
            if (day >= 10)
            {
                date += day;
            }
            else
            {
                date += "0" + day;
            }

            return date;
        }

        public static (long, double) binarySearch(long unixTime, List<(long, double)> stockValues)
        {
            long number =unixTime;
            List<(long, double)> trialList = stockValues;
            int min = 0;
            int max = trialList.Count - 1;
            bool exitLoop = false;

            while (!exitLoop)
            {
                int midPoint = (min + max) / 2;

                if (number == trialList[midPoint].Item1)
                {
                    return trialList[midPoint];
                }
                else if(min >= max)
                {
                    if (max < 0)
                    {
                        return trialList[min];
                    }
                    else
                    {
                        return trialList[max];
                    }
                }
                else if (number > trialList[midPoint].Item1)
                {
                    min = midPoint+1;
                }
                else if(number < trialList[midPoint].Item1)
                {
                    max = midPoint-1;
                }   
            }
            return trialList[max];
        }
        public static void DisplayTable(List<List<(long, double)>> prices, List<string> stockNames,List<List<double>> coeffcients)
        {
            List<(long, double)> userDefinedDates = new List<(long, double)>();
            int count = 1;
            bool exitLoop = false;

            while(!exitLoop)
            {
                Console.Write("Would you like to find the price for a certain day? (y/n)");

                if (Console.ReadLine() == "y")
                {
                    userDefinedDates.Add(binarySearch(ConvertToUNIXMilli(), prices[0]));
                }
                else
                {
                    exitLoop = true;
                }
            }

            Console.WriteLine("Now setting when to see predicted values until.");
            long endOfPrediction = ConvertToUNIXMilli();


            Console.Clear();
            Console.Write("Date");

            for(int i = 0;i < stockNames.Count;i++)
            {
                Console.SetCursorPosition(5+(i+1)*8,0);
                Console.Write("| " + stockNames[i]);
            }

            for (int i = 0; i < prices[0].Count; i += prices[0].Count / 20)
            {
                Console.SetCursorPosition(0, count);
                Console.Write(ConvertToyyyymmdd(prices[0][i].Item1));
                for (int j = 0; j < prices.Count; j++)
                {
                    Console.SetCursorPosition(5 + (j + 1) * 8, count);

                    string buffer = Math.Round(double.Parse(prices[j][i].Item2.ToString()), 2).ToString();
                    if (buffer[buffer.Length - 2] == '.')
                    {
                        Console.Write("| " + buffer + "0");
                    }
                    else
                    {
                        Console.Write("| " + buffer);
                    }
                }

                count++;
            }
            for (int i = 0; i < userDefinedDates.Count; i ++)
            {
                Console.SetCursorPosition(0, count);
                Console.Write(ConvertToyyyymmdd(prices[0][i].Item1));
                for (int j = 0; j < prices.Count; j++)
                {
                    Console.SetCursorPosition(5 + (j + 1) * 8, count);

                    string buffer = Math.Round(double.Parse(userDefinedDates[i].Item2.ToString()), 2).ToString();
                    if (buffer[buffer.Length - 2] == '.')
                    {
                        Console.Write("| " + buffer + "0");
                    }
                    else
                    {
                        Console.Write("| " + buffer);
                    }
                }
            }


            Console.WriteLine("\nPredicted values below");
            count++;
            for (long i = prices[0][prices[0].Count-1].Item1; i <endOfPrediction; i += 86400000*20)
            {
                Console.SetCursorPosition(0, count);
                Console.Write(ConvertToyyyymmdd(i));

                for (int j = 0; j < prices.Count; j++)
                {
                    Console.SetCursorPosition(5 + (j + 1) * 8, count);

                    string buffer = FOfX(i,coeffcients[j]).ToString();
                    if (buffer[buffer.Length - 2] == '.')
                    {
                        Console.Write("| " + buffer + "0");
                    }
                    else
                    {
                        Console.Write("| " + buffer);
                    }
                }

                count++;
            }
            Console.WriteLine();
        }
        public static double FOfX(long x, List<double> curve)
        {
            double sum = 0;

            for (int i = 0; i < curve.Count; i++)
            {
                sum += curve[i] * double.Parse(Math.Pow(x, i).ToString());
            }

            return sum;
        }
    }
} 