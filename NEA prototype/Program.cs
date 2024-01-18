using System;
using System.Collections.Generic;
using System.IO; 
using System.Net; 
using Newtonsoft.Json; 
using System.Text.RegularExpressions;
using System.Linq;

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
        //vw is the volume weighted average price
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            PolynomialRegression p = new PolynomialRegression();

            List<string> stockNames = new List<string>();
            List<List<(long, double)>> stockPrices = new List<List<(long, double)>>();
            
            int numOfStocks = 0;

            Console.WriteLine("Welcome to SPAM");

            while (true)
            {
                string r = "^[1-8]$";

                Console.WriteLine("How many stocks would you like to graph?");
                string buffer = Console.ReadLine();
                if (Regex.IsMatch(buffer, r))
                {
                    numOfStocks = int.Parse(buffer);
                    break;
                }
            }

            long offset = 0;

            for (int i = 0; i < numOfStocks; i++)
            {
                InfoAboutStock infoAboutStock = new InfoAboutStock();
                List<(long, double)> points = new List<(long, double)>();
                while (true)
                {
                    infoAboutStock = DoAPIRequest(TypeOfAnalysis());
                    prices[] valuesOfStock = infoAboutStock.results;
                    points = new List<(long, double)>();

                    if (valuesOfStock.Length > 0)
                    {
                        long l = valuesOfStock[0].t / 1000 - 86400;
                        offset = l;
                        foreach (prices pr in valuesOfStock)
                        {
                            points.Add((pr.t / 1000 - l, pr.vw));
                        }
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid ticker for the NASDAQ in the correct capitalization");
                    }
                }
                stockNames.Add(infoAboutStock.ticker);
                stockPrices.Add(points);
            }

            int howToDisplay = HowToDisplay();
            
            List<List<double>> listOfCurves = new List<List<double>>();
            
            for (int i = 0; i < stockNames.Count; i++)
            {
                listOfCurves.Add(p.DoPolynomialRegression(stockPrices[i]));
            }

            Line_Chart GraphOfStockValue = new Line_Chart(offset);

            if (howToDisplay != 2)
            {
                for (int i = 0; i < stockNames.Count; i++)
                {
                    GraphOfStockValue.AddNewSeries(stockPrices[i], stockNames[i]);
                    GraphOfStockValue.AddRegressionCurve(listOfCurves[i], 0, stockNames[i]);
                }
                GraphOfStockValue.ShowDialog();
            }

            if (howToDisplay != 1)
            {
                DisplayTable(stockPrices, stockNames, listOfCurves, offset);
            }
            

            Console.WriteLine("Goodbye");
            Console.ReadKey();
        }
        static int TypeOfAnalysis()
        {
            while (true)
            {
                string r = "1|2";

                while (true)
                {
                    Console.WriteLine("Would you like to do a new analysis (1) or use example analysis (2)");
                    string buffer = Console.ReadLine();

                    if (Regex.IsMatch(buffer, r))
                    {
                        return int.Parse(buffer);
                    }
                    else
                    {
                        Console.WriteLine("Please enter a response in the correct format --> 1 or 2");
                    }
                }
            }
        }
        static int HowToDisplay()
        {
            while (true)
            {
                string r = "1|2|3";

                Console.WriteLine("How would you like to get the data?\nJust graph - 1\nJust table - 2\nBoth - 3");
                string buffer = Console.ReadLine();

                if (Regex.IsMatch(buffer, r))
                {
                    return int.Parse(buffer);
                }
                else
                {
                    Console.WriteLine("Please enter either 1, 2 or 3");
                }
            }
        }
        static InfoAboutStock DoAPIRequest(int choice)
        {
            InfoAboutStock infoAboutStock = new InfoAboutStock();
            
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
                        string exampleAddress = "https://api.polygon.io/v2/aggs/ticker/NVDA/range/1/day/2023-01-01/2024-01-01?adjusted=true&sort=asc&limit=5000&apiKey=CM_QQuAvxVCV7hM8RS9jDCRIJh85Ux2v";
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
        static string SetUpRequest()
        {
            string output = "";
            
            string[] s = DateTime.Now.Date.ToString().Split(' ');
            string[] currentDay = s[0].Split('/');
            long maxUNIX = ConvertToUNIX(currentDay[2] + "-" + currentDay[1] + "-" + currentDay[0]);
            long minUNIX = maxUNIX - 63158400;
            
            while (true)
            {
                output = "https://api.polygon.io/v2/aggs/ticker/";
                string regExDate = "^202\\d-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])$";
                string regExTicker = "\\w+";

                while (true)
                {
                    Console.WriteLine("Please enter the ticker for the stock on the NASDAQ.");
                    string buffer = Console.ReadLine();

                    if (Regex.IsMatch(buffer, regExTicker))
                    {
                        output += buffer + "/range/1/";
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Ticker in incorrect format");
                    }
                }

                while (true)
                {
                    Console.WriteLine("Please enter the time span for the request.\nThe only accepted time spans are: minute, hour, day, week, month, quarter, year.");
                    string buffer = Console.ReadLine();
                    List<string> timeSpans = new List<string> { "minute", "hour", "day", "month", "quarter", "year" };
                    if (timeSpans.Contains(buffer))
                    {
                        output += buffer + "/";
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Time span in incorrect format");
                    }
                }

                string dates = "";

                while (true)
                {
                    long startDate = 0;
                    long endDate = 0;

                    while (true)
                    {
                        Console.WriteLine("Please enter the start date for the analysis.\nPlease enter all dates in the form yyyy-mm-dd.");
                        string buffer = Console.ReadLine();

                        if (Regex.IsMatch(buffer, regExDate))
                        {
                            dates += buffer + "/";
                            startDate = ConvertToUNIX(buffer);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Date in incorrect format or is out of range");
                        }
                    }

                    while (true)
                    {
                        Console.WriteLine("Please enter the end date for the analysis.\nPlease enter all dates in the form yyyy-mm-dd.");
                        string buffer = Console.ReadLine();
                        if (Regex.IsMatch(buffer, regExDate))
                        {
                            dates += buffer;
                            endDate = ConvertToUNIX(buffer);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Date in incorrect format or is out of range");
                        }
                    }

                    if (startDate < endDate && startDate > minUNIX && endDate < maxUNIX)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("The end date needs to be after the start date\nDates can't be more then 2 years in the past\nDates can't be in the future");
                    }
                }
                output += dates + "?adjusted=true&sort=asc&limit=5000&apiKey=CM_QQuAvxVCV7hM8RS9jDCRIJh85Ux2v";

                while (true)
                {
                    Console.WriteLine("The API request is: " + output);

                    Console.WriteLine("Is this correct?\ny or n");

                    string response = Console.ReadLine();

                    if (response == "y")
                    {
                        return output;
                    }
                    else if (response == "n")
                    {
                        Console.WriteLine("Remaking API url");
                        Console.Clear();

                        break;
                    }
                    else
                    {
                        Console.WriteLine("Please enter a response in the correct format --> lowercase y or n");
                    }
                }
            }
        }
        public static long ConvertToUNIX()
        {
            string regExDate = "^202\\d-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])$";

            while (true)
            {
                Console.WriteLine("Please enter the date.\nPlease enter all dates in the form yyyy-mm-dd.");
                string buffer = Console.ReadLine();

                if (Regex.IsMatch(buffer, regExDate))
                {
                    return Conversion(buffer);
                }
                else
                {
                    Console.WriteLine("date in incorrect format or is out of range");
                }
            }
        }
        public static long ConvertToUNIX(string date)
        {
            return Conversion(date);
        }
        static long Conversion(string date)
        {
            long unix = 0;
            string[] separated = new string[3];

            separated = date.Split('-');

            for (int i = 1971; i <= int.Parse(separated[0]); i++)
            {
                if (i % 4 == 0)
                {
                    unix += 86400 * 366;
                }
                else
                {
                    unix += 86400 * 365;
                }
            }

            long[] daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            for (int i = 0; i < int.Parse(separated[1]) - 1; i++)
            {
                unix += daysInMonth[i] * 86400;
            }

            unix += (long.Parse(separated[2]) - 1) * 86400;

            return unix;
        }
        public static string ConvertToyyyymmdd(long unixTime)
        {
            unixTime -= 4 * 60 * 60;
            string date = "";

            int year = 1970;
            int month = 1;
            int day = 0;
            int count = 2;
            long[] leapYear = { 1,0,0,0 };

            while (true)
            {
                if(unixTime - 86400 * 365 - leapYear[count%4] < 0)
                {
                    break;
                }
                else
                {
                    unixTime -= 86400 * 365 + leapYear[count % 4];
                    year++;
                    count++;
                }
            }
           
            count = 0;

            long[] daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            while (true)
            {
                if (unixTime - daysInMonth[count] * 86400 < 0)
                {
                    break;
                }
                else
                {
                    unixTime -= daysInMonth[count] * 86400;
                    count++;
                    month++;
                }
            }

            

            while (unixTime > 0)
            {
                unixTime -= 24 * 3600;
                day++;
            }

            if (day == 0)
            {
                day++;
            }

            date += year + "-";

            if (month >= 10)
            {
                date += month + "-";
            }
            else
            {
                date += "0" + month + "-";
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
        static (long, double) binarySearch(long unixTime, List<(long, double)> stockValues)
        {
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
        }
        static void DisplayTable(List<List<(long, double)>> prices, List<string> stockNames,List<List<double>> coefficients, long offset)
        {
            List<(long, double)> userDefinedDates = new List<(long, double)>();

            int count = 1;

            while(true)
            {
                Console.Write("Would you like to find the price for a certain day? (y/n)");

                if (Console.ReadLine() == "y")
                {
                    userDefinedDates.Add(binarySearch(ConvertToUNIX()+offset, prices[0]));
                }
                else
                {
                    break;
                }
            }
            long endOfPrediction = 0;
            string[] s = DateTime.UtcNow.Date.ToString().Split(' ');
            string currentDay = s[0].Replace('/', '-');
            long maxUNIX = ConvertToUNIX(currentDay);
            while (endOfPrediction >maxUNIX)
            {
                Console.WriteLine("Now setting when to see predicted values until.\nPlease make sure to enter a date in the future");
                endOfPrediction = ConvertToUNIX();
            }

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
                Console.Write(ConvertToyyyymmdd(prices[0][i].Item1+offset));
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
                Console.Write(ConvertToyyyymmdd(userDefinedDates[i].Item1 + offset));
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
                count++;
            }

            Console.WriteLine("\nPredicted values below");
            count++;

            for (long i = prices[0][prices[0].Count-1].Item1+offset; i <endOfPrediction; i += 86400*20)
            {
                Console.SetCursorPosition(0, count);
                Console.Write(ConvertToyyyymmdd(i));

                for (int j = 0; j < prices.Count; j++)
                {
                    Console.SetCursorPosition(5 + (j + 1) * 8, count);

                    string buffer = FOfX(i, coefficients[j]).ToString();

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
        static double FOfX(long x, List<double> curve)
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