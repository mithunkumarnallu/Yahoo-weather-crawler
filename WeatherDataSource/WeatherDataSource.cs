using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Runtime.InteropServices;
using System.IO;

namespace DataSource
{
    public class WeatherDataSource : IWeatherDataSource
    {
        const string WEATHER_URL = "http://in.weather.yahoo.com/india";
        const string WEATHER_BASE_URL = "http://in.weather.yahoo.com";
        const string CELCIUS_CLASS = "day-temp-current temp-c ";
        const string FARHENHEIT_CLASS = "day-temp-current temp-f ";

        public void LoadDataSource()
        {
            try
            {
                WebClient webClient = new WebClient();
                decimal tempInDegree;
                decimal tempInFahrenheit;

                Dictionary<string, string> stateLinkMap = null;
                List<Dictionary<string, string>> cityLinkList = new List<Dictionary<string, string>>();
                Dictionary<string, string> cityLinkMap = null;

                //RemoveWeatherData();

                string htmlString = webClient.DownloadString(WEATHER_URL);
                stateLinkMap = LinkHelper.GetLinks(htmlString, "page1");
                //StringBuilder sb = new StringBuilder();

                foreach (KeyValuePair<string, string> kvp in stateLinkMap)
                {
                    htmlString = webClient.DownloadString(WEATHER_BASE_URL + kvp.Value);
                    cityLinkMap = LinkHelper.GetLinks(htmlString, "page1");


                    foreach (KeyValuePair<string, string> cityLink in cityLinkMap)
                    {

                        ///sb.Append(WEATHER_BASE_URL + cityLink.Value + "\n");
                        htmlString = webClient.DownloadString(WEATHER_BASE_URL + cityLink.Value);
                        tempInDegree = LinkHelper.GetDataFromHTMLText(htmlString, CELCIUS_CLASS);

                        tempInFahrenheit = LinkHelper.GetDataFromHTMLText(htmlString, FARHENHEIT_CLASS);

                        AddWeatherData(kvp.Key, cityLink.Key, tempInDegree, tempInFahrenheit);
                    }
                }
                //File.WriteAllText(@"E:\Studies\MSIT Fall 2014\Data Science\Homeworks\Yahoo weather crawler\provenance.txt", sb.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetData(string state = null, string city = null)
        {
            
                using (SqlCeConnection con = new SqlCeConnection("Data Source='" + AppDomain.CurrentDomain.BaseDirectory + "WeatherDB.sdf';"))
                {
                    con.Open();
                    DataTable result = new DataTable();
                    try
                    {
                        if (state == null)
                        {
                            result.Columns.Add("State");
                            SqlCeCommand cmd = new SqlCeCommand("Select distinct State from WeatherTable", con);
                            SqlCeDataReader rdr = cmd.ExecuteReader();

                            try
                            {
                                // Iterate through the results
                                //
                                while (rdr.Read())
                                {
                                    result.Rows.Add(new object[]{rdr.GetString(0)});
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        else if (city == null)
                        {
                            result.Columns.Add("State");
                            result.Columns.Add("City");
                            SqlCeCommand cmd = new SqlCeCommand("Select State, City from WeatherTable where State='" + state + "'", con);
                            SqlCeDataReader rdr = cmd.ExecuteReader();

                            try
                            {
                                // Iterate through the results
                                //
                                while (rdr.Read())
                                {
                                    result.Rows.Add(new object[] { rdr.GetString(0), rdr.GetString(1) });
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        else
                        {
                            result.Columns.Add("State");
                            result.Columns.Add("City");
                            result.Columns.Add("WeatherInDegrees");
                            result.Columns.Add("WeatherInFarenheit");
                            SqlCeCommand cmd = new SqlCeCommand("Select State, City, WeatherInDegrees, WeatherInFarenheit  from WeatherTable where State='" + state + "' and City='" + city + "'", con);
                            SqlCeDataReader rdr = cmd.ExecuteReader();

                            try
                            {
                                // Iterate through the results
                                //
                                while (rdr.Read())
                                {
                                    result.Rows.Add(new object[] { rdr.GetString(0), rdr.GetString(1), rdr.GetValue(2), rdr.GetValue(3)});
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        con.Close();
                    }
                return result;
            }   
        }

        private void RemoveWeatherData()
        {
            using (SqlCeConnection con = new SqlCeConnection("Data Source='" + AppDomain.CurrentDomain.BaseDirectory + "WeatherDB.sdf';"))
            {
                con.Open();
                try
                {
                    using (SqlCeCommand command = new SqlCeCommand(
                    "Delete from WeatherTable", con))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        private void AddWeatherData(string state, string city, decimal tempInDegree, decimal tempInFahrenheit)
        {
            using (SqlCeConnection con = new SqlCeConnection("Data Source='" + AppDomain.CurrentDomain.BaseDirectory + "WeatherDB.sdf';"))
            {
                con.Open();
                try
                {
                    using (SqlCeCommand command = new SqlCeCommand(
                    "INSERT INTO WeatherTable VALUES(@State, @City, @WeatherInDegrees, @WeatherInFarenheit, @DateCreatedInGMT)", con))
                    {
                        //command.Parameters.Add(new SqlCeParameter("DateCreatedInGMT", DateTime.Now.ToUniversalTime().ToString()));                        
                        command.Parameters.Add(new SqlCeParameter("State", state));
                        command.Parameters.Add(new SqlCeParameter("City", city));
                        command.Parameters.Add(new SqlCeParameter("WeatherInDegrees", float.Parse(tempInDegree.ToString())));
                        command.Parameters.Add(new SqlCeParameter("WeatherInFarenheit", float.Parse(tempInFahrenheit.ToString())));
                        command.Parameters.Add(new SqlCeParameter("DateCreatedInGMT", DateTime.Now.ToUniversalTime().ToString("r")));                        
                        command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    Console.WriteLine("Count not insert.");
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}
