using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSource;
using System.Data;

namespace YahooWeatherWebCrawler
{
    class WebApplication
    {
        static WeatherDataSource _weatherDS = new WeatherDataSource();

        public static WeatherDataSource WeatherDS
        {
            get { return WebApplication._weatherDS; }
            set { WebApplication._weatherDS = value; }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Weather data loading is to be done. Do you want to continue loading or work with existing data?\nY - Load\tN - Use old data");
            string userOption= Console.ReadLine().ToLower();
            string secUserOption;

            while (userOption != "y" && userOption != "n")
            {
                Console.WriteLine("Please enter valid option");
                userOption= Console.ReadLine().ToLower();
            }

            if (userOption == "y")
            {
                Console.WriteLine("Loading Weather data from the web...");
                WeatherDS.LoadDataSource();
                Console.WriteLine("Weather data loaded from the web. Thanks for your patience!!");
            }

            while (userOption != "3")
            {
                Console.WriteLine("What next??\n 1 - Load Data\n 2 - Perform Search\n 3 - Quit");
                userOption = Console.ReadLine();
                switch (userOption)
                {
                    case "1":
                        Console.WriteLine("Loading Weather data from the web...");
                        WeatherDS.LoadDataSource();
                        Console.WriteLine("Weather data loaded from the web. Thanks for your patience!!");
                        break;

                    case "2":
                        DisplayData();
                        break;
                    case "3":
                        return;
                }
            }


        }

        static void DisplayData()
        {
            Console.WriteLine("Displaying all States..");
                        
            DataTable data = WeatherDS.GetData(null,null);
            string userOption;
            string state = null, city = null;

            StringBuilder displayData = new StringBuilder();
            int counter = 1;

            foreach(DataRow dr in data.Rows)
            {
                displayData.Append(counter + " " + dr["State"] + "\n");
                counter++;
            }
            Console.WriteLine(displayData);
            userOption = (data.Rows.Count + 1).ToString();
            
            while (int.Parse(userOption) > data.Rows.Count)
            {
                Console.WriteLine("Please select a state from above...");
                userOption = Console.ReadLine();
            }

            state = data.Rows[int.Parse(userOption) - 1]["State"].ToString();

            data = WeatherDS.GetData(data.Rows[int.Parse(userOption)-1]["State"].ToString());
            
            displayData = new StringBuilder();
            counter = 1;

            foreach (DataRow dr in data.Rows)
            {
                displayData.Append(counter + " " + dr["City"] + "\n");
                counter++;
            }
            Console.WriteLine(displayData);
            userOption = (data.Rows.Count + 1).ToString();

            while (int.Parse(userOption) > data.Rows.Count)
            {
                Console.WriteLine("Please select a City from above...");
                userOption = Console.ReadLine();
            }
            city = data.Rows[int.Parse(userOption)-1]["City"].ToString();
            data = WeatherDS.GetData(state, city);

            displayData = new StringBuilder();
            counter = 1;

            foreach (DataRow dr in data.Rows)
            {
                displayData.Append("Weather at "+city+" in Degrees - " + dr["WeatherInDegrees"] + "\n");
                displayData.Append("Weather at "+city+" in Fahrenheit - " + dr["WeatherInFarenheit"] + "\n");
                counter++;
            }
            Console.WriteLine(displayData);
        }
    }
}
