using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Engineer;

namespace EngineerLib
{

    /// <summary>
    /// A data class to handle multiple Entities.
    /// </summary>
    public class EngineerLibDataClass
    {
        public List<EngineerLibDataEntity> entities;

        public EngineerLibDataClass()
        {
            entities = new List<EngineerLibDataEntity>();
        }
    }
    /// <summary>
    /// A class to PARSE data and COMPILE the game.
    /// </summary>
    public class EngineerLibParser
    {
        public EngineerLibParser()
        {

        }
        /// <summary>
        /// Parses a given string into a list of Parsed EngineerLibDataEntities.
        /// </summary>
        /// <param name="input">The given string of code.</param>
        /// <returns>A list of parsed EngineerLibDataEntities</returns>
        public static int getIndex(List<string> vs, string match)
        {
            int i = 0;
            int index = 0;

            foreach(string lol in vs)
            {
                if(lol == match)
                {
                    i = index;
                }
                else
                {
                    index++;
                }
            }

            return i;
        }
        public static string parseData(string input, Func<string, int, string> logString)
        {
            List<EngineerLibDataEntity> result = new List<EngineerLibDataEntity>();
            List<string> lines = (input + "\n").Split('\n').ToList();
            int errors = 0;

            Regex matchOptions = new Regex(@" *\([a-z]+\)\[\d+\]:");
            MatchCollection matches = matchOptions.Matches(input);

            logString("Loading libraries...", 0);
            string[] libraries = { "System", "System.Collections.Generic", 
                "System.Linq", "System.Runtime.Serialization", 
                "System.Text", "System.Text.ReguralExpressions", 
                "System,Threading.Tasks", "System.Windows.Forms", 
                "Newtonsoft.Json", "Engineer"};
            foreach(string library in libraries)
            {
                logString("Loaded " + library, 0);
            }

            logString("Starting the parser...", 0);
            foreach (Match resultMatch in matches)
            {
                logString(resultMatch.Value, -1);
                EngineerLibDataEntity item = new EngineerLibDataEntity();
                string type = new Regex(@"\([a-z]+\)").Match(resultMatch.Value).Value.Substring(1, new Regex(@"\([a-z]+\)").Match(resultMatch.Value).Value.Length - 2);
                double id = Int64.Parse(new Regex(@"\[\d+\]").Match(resultMatch.Value).Value.Substring(1, new Regex(@"\[\d+\]").Match(resultMatch.Value).Value.Length - 2));
                List<string> types = new string[] { "section", "dialog", "choice" }.ToList();

                item.ID = id;
                item.type = type;
                int i = getIndex(lines, resultMatch.Value) + 1;
                logString("Parsing element with ID " + id + "...", i);
                if (types.IndexOf(type) > -1)
                {
                    while ((lines[i].ToLower().IndexOf("end") == -1) && (lines.Count > i - 1) && (lines[i].Length > 1))
                    {
                        EngineerLibDataEntityAttribute line = new EngineerLibDataEntityAttribute();
                        Regex attributeRegex = new Regex(@"[	 ]+\<[a-z]+\>\:.+");
                        Regex typeRegexUnfiltered = new Regex(@"[	 ]+\<[a-z]+\>\:");
                        Regex typeRegex = new Regex(@"\<[a-z]+\>");

                        if (attributeRegex.Match(lines[i]).Success)
                        {
                            if (typeRegexUnfiltered.Match(lines[i]).Success)
                            {
                                string attribute = attributeRegex.Match(lines[i]).Value;
                                string lineType = typeRegex.Match(lines[i]).Value;
                                lineType = lineType.Substring(1, lineType.Length - 2);
                                string lineTypeUnfiltered = typeRegexUnfiltered.Match(lines[i]).Value;
                                string data = Regex.Replace(attribute, lineTypeUnfiltered, "");

                                line.type = lineType;
                                line.value = data;

                                item.attributes.Add(line);
                                logString("Attribute creation successsful " + typeRegex.Match(lines[i]).Value, i);

                            }
                            else
                            {
                                logString("Error: The data attribute is invalid.", i);
                                errors++;
                            }
                        }
                        else
                        {
                            logString("Error: The data line didn't contain any valid data.", i);
                            errors++;
                        }

                        i++;
                    }
                    logString("Finished parsing element with ID " + id + "...", i);

                    result.Add(item);
                }
                else
                {
                    logString("Error: Invalid attribute type", i);
                }


            }

            logString("Finished parsing.", 0);
            EngineerLibDataClass engineerLibDataClass = new EngineerLibDataClass();
            engineerLibDataClass.entities = result;
            if(errors == 0)
            {
                return JsonConvert.SerializeObject(engineerLibDataClass);
            }
            else
            {
                return "ERROR";
            }
        }
    }
    public class EngineerLibDataEntityAttribute
    {
        public string type;
        public string value;
    }

    public class EngineerLibDataEntity
    {
        public string type;
        public double ID;
        public List<EngineerLibDataEntityAttribute> attributes;

        public EngineerLibDataEntity()
        {
            ID = 0;
            attributes = new List<EngineerLibDataEntityAttribute>();
        }
    }

    [Serializable]
    internal class EngineerLibDataParsingFailureException : Exception
    {
        public EngineerLibDataParsingFailureException()
        {

        }

        public EngineerLibDataParsingFailureException(string message) : base(message)
        {
            // MessageBox.Show("Failed to parse game code, Error:\n" + message);
        }

        public EngineerLibDataParsingFailureException(string message, Exception innerException) : base(message, innerException)
        {
            // MessageBox.Show("Failed to parse game code, Error:\n" + message);
        }
    }

    static class LevenshteinDistance
    {
        public static int Compute(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }
    }
}
