using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace DataSource
{
    class LinkHelper
    {
        public static Dictionary<string,string> GetLinks(string htmlString, string divId = null)
        {
            try
            {
                Dictionary<string, string> links = new Dictionary<string, string>();
                MatchCollection matchLinkColl = null;
                MatchCollection matchSpanColl = null;

                string regexString = "<div id=\"" + divId + "\"(.*?)</div>";

                MatchCollection divMatches = Regex.Matches(htmlString, regexString,
                                    RegexOptions.Singleline);
                foreach (Match divMatch in divMatches)
                {
                    string value = divMatch.Groups[1].Value;
                    matchLinkColl = Regex.Matches(value, @"href=\""(.*?)\""", RegexOptions.Singleline);
                    matchSpanColl = Regex.Matches(value, @"<span>(.*?)</span>", RegexOptions.Singleline);

                    for (int counter = 0; counter < matchLinkColl.Count; counter++)
                    {
                        links.Add(matchSpanColl[counter].Groups[1].Value, matchLinkColl[counter].Groups[1].Value);
                    }
                }
                return links;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static decimal GetDataFromHTMLText(string htmlString, string classId)
        {
            try
            {
                string regexString = "<div class=\"" + classId + "(.*?)<span class=\"unit\">";
                decimal result = decimal.MaxValue;
                
                MatchCollection classMatches = Regex.Matches(htmlString, regexString,
                                    RegexOptions.Singleline);

                if(classMatches.Count > 0)
                {
                    string value = classMatches[0].Groups[1].Value;
                    regexString = @"[0-9]+\.[0-9]+";
                    Match match = Regex.Match(value, regexString);
                    if (match.Value == "")
                    {
                        regexString = @"\d+";
                        match = Regex.Match(value, regexString);
                    }
                    result = decimal.Parse(match.Value);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
