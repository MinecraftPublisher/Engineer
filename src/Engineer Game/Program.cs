using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EngineerLib;
using System.IO;
using System.Text.RegularExpressions;

namespace Engineer_Game
{
    class Program
    {
        static string data = "DATA_GAME_JSON";
        static EngineerLibDataClass lib = JsonConvert.DeserializeObject<EngineerLibDataClass>(data);
        static EngineerLibDataEntity[] entities = lib.entities.ToArray();
        static void Main(string[] args)
        {
            Console.Title = EngineerGameData.name + " v" + EngineerGameData.version + " made by " + EngineerGameData.creator;
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string dirPath = Path.Combine(appData, EngineerGameData.name);
            string savePath = Path.Combine(dirPath, "save.txt");
            Directory.CreateDirectory(Path.Combine(appData, EngineerGameData.name));
            int index = 0;
            if (File.Exists(savePath))
            {
                EngineerSystem.Type("Welcome back, preparing the game...", EngineerGameData.debugMode, 10, 10, 10);
                if(Int32.TryParse(File.ReadAllText(savePath), out index))
                {

                }
                else
                {
                    EngineerSystem.Type("Curropted data detected, resetting...", EngineerGameData.debugMode, 10, 10, 10);
                }
            }
            else
            {
                EngineerSystem.Type("Preparing game...", EngineerGameData.debugMode, 10, 10, 10);
            }
            for (int i = index; i < entities.Length; i++)
            {
                if(entities[i].type == "section")
                {
                    LaunchEntity(entities[i], EngineerGameData.debugMode);
                    EngineerSystem.Type("Autosave....", EngineerGameData.debugMode, 100);
                    File.WriteAllText(savePath, i.ToString());
                    EngineerSystem.Type("Save success.", EngineerGameData.debugMode, 100);
                }
            }

            while(true)
            {

            }
        }

        static EngineerLibDataEntity getEntityWithId(EngineerLibDataEntity[] entities, double id)
        {
            EngineerLibDataEntity result = new EngineerLibDataEntity();

            foreach(EngineerLibDataEntity entity in entities)
            {
                if(entity.ID == id)
                {
                    result = entity;
                    break;
                }
            }

            return result;
        }

        static void LaunchEntity(EngineerLibDataEntity entity, bool isDebug)
        {
            string Entitytype = entity.type;
            EngineerLibDataEntityAttribute[] attributes = entity.attributes.ToArray();
            if(Entitytype == "section")
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    if(attributes[i].type == "ref" && Regex.IsMatch(attributes[i].value, @"\[\d+\]"))
                    {
                        try
                        {
                            double id = Int64.Parse(attributes[i].value.Substring(1, attributes[i].value.Length - 2));
                            LaunchEntity(getEntityWithId(entities, id), isDebug);
                        }
                        catch
                        {
                            Console.WriteLine("Error: failed to read ID ( Close after 5 seconds )");
                            Thread.Sleep(5000);
                            Environment.Exit(0);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: invalid attribute type given in a SECTION ( Close after 5 seconds )\nATTRIBUTE TYPE: \"" + attributes[i].type + "\"\nATTRIBUTE VALUE: \"" + attributes[i].value + "\"");
                        Thread.Sleep(5000);
                        Environment.Exit(0);
                    }
                }
            }
            else if(Entitytype == "responsive")
            {
                string text = "";
                string variableName = "";
                for (int i = 0; i < attributes.Length; i++)
                {
                    if (attributes[i].type == "text" && Regex.IsMatch(attributes[i].value, "\'.+\'") && text == "")
                    {
                        try
                        {
                            text = attributes[i].value.Substring(1, attributes[i].value.Length - 2);
                        }
                        catch
                        {
                            Console.WriteLine("Error: failed to read TEXT ( Close after 5 seconds )");
                            Thread.Sleep(5000);
                            Environment.Exit(0);
                        }
                    }
                    else if(attributes[i].type == "text" && Regex.IsMatch(attributes[i].value, "\'.+\'") && text != "")
                    {
                        Console.WriteLine("Attribute TEXT already defined ( Close after 5 seconds )");
                        Thread.Sleep(5000);
                        Environment.Exit(0);
                    }
                    else if(attributes[i].type == "name" && Regex.IsMatch(attributes[i].value, "\'.+\'") && variableName == "")
                    {
                        variableName = attributes[i].value.Substring(1, attributes[i].value.Length - 2);
                    }
                    else if (attributes[i].type == "name" && Regex.IsMatch(attributes[i].value, "\'.+\'") && variableName != "")
                    {
                        Console.WriteLine("Attribute NAME already defined ( Close after 5 seconds )");
                        Thread.Sleep(5000);
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("Error: invalid attribute type given in a RESPONSIVE ( Close after 5 seconds )\nATTRIBUTE TYPE: \"" + attributes[i].type + "\"\nATTRIBUTE VALUE: \"" + attributes[i].value + "\"");
                        Thread.Sleep(5000);
                        Environment.Exit(0);
                    }

                    Console.Write(text);
                    File.WriteAllText(EngineerSystem.makeSavePath("variables", variableName), Console.ReadLine());
                }
            }
            else if (Entitytype == "dialog")
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    if (attributes[i].type == "text" && Regex.IsMatch(attributes[i].value, "\'.+\'"))
                    {
                        string data = attributes[i].value.Substring(1, attributes[i].value.Length - 2);
                        EngineerSystem.Type(EngineerSystem.includeVariables(data), isDebug);
                    }
                    else
                    {
                        Console.WriteLine("Error: invalid attribute type given in a DIALOG ( Close after 5 seconds )\nATTRIBUTE TYPE: \"" + attributes[i].type + "\"\nATTRIBUTE VALUE: \"" + attributes[i].value + "\"");
                        Thread.Sleep(5000);
                        Environment.Exit(0);
                    }
                }
            }
            else if(Entitytype =="choice")
            {
                if (attributes[0].type == "text" && Regex.IsMatch(attributes[0].value, "\'.+\'"))
                {
                    string data = attributes[0].value.Substring(1, attributes[0].value.Length - 2);
                    EngineerSystem.Type(EngineerSystem.includeVariables(data), isDebug);
                    List<EngineerChoiceOption> options = new List<EngineerChoiceOption>();

                    for (int i = 1; i < attributes.Length; i++)
                    {
                        if (attributes[i].type == "option" && Regex.IsMatch(attributes[i].value, @"\[\'.+\'\,\d+\]"))
                        {
                            try
                            {
                                string cuttedValue = attributes[i].value.Substring(1, attributes[i].value.Length - 2);
                                string uncuttedText = cuttedValue.Split(',')[0];
                                string cuttedText = uncuttedText.Substring(1, uncuttedText.Length - 2);
                                string reference = cuttedValue.Split(',')[1];

                                options.Add(new EngineerChoiceOption(EngineerSystem.includeVariables(cuttedText), reference));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error: invalid attribute value given in a CHOICE OPTION ( Close after 10 seconds )\nATTRIBUTE VALUE: \"" + attributes[i].value + "\"\nPROGRAM ERROR:\n" + ex);
                                Thread.Sleep(10000);
                                Environment.Exit(0);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error: invalid attribute type given in a CHOICE ( Close after 5 seconds )\nATTRIBUTE TYPE: \"" + attributes[i].type + "\"\nATTRIBUTE VALUE: \"" + attributes[i].value + "\"");
                            Thread.Sleep(5000);
                            Environment.Exit(0);
                        }
                    }

                    LaunchEntity(getEntityWithId(entities, Int64.Parse(EngineerSystem.ChoiceSync(options.ToArray(), isDebug).reference)), isDebug);
                }
                else
                {
                    Console.WriteLine("Error: TEXT attribute not found at the start of a CHOICE. ( Close after 5 seconds )\nATTRIBUTE TYPE: \"" + attributes[0].type + "\"\nATTRIBUTE VALUE: \"" + attributes[0].value + "\"");
                }
            }
            else
            {
                Console.WriteLine("Error: invalid entity data type, is the data curropted? ( Close after 5 seconds )\nENTITY TYPE: \"" + attributes[0].type + "\"\nENTITY VALUE: \"" + attributes[0].value + "\"");
                Thread.Sleep(5000);
                Environment.Exit(0);
            }
        }
    }

}

namespace EngineerLib
{
    public class EngineerGameData
    {
        public static string name = "DATA_GAME_NAME";
        public static string version = "DATA_GAME_VERSION";
        public static string creator = "DATA_GAME_OWNER";
        public static bool debugMode = "DATA_GAME_DEBUG".Equals("true");
    }
    public class EngineerLibDataClass
    {
        public List<EngineerLibDataEntity> entities;

        public EngineerLibDataClass()
        {
            entities = new List<EngineerLibDataEntity>();
        }
    }
    public struct EngineerChoiceOption
    {
        public string text;
        public string reference;

        public EngineerChoiceOption(string text, string reference)
        {
            this.text = text;
            this.reference = reference;
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

    class EngineerSystem
    {
        /// <summary>
        /// Used to animate a typing style.
        /// </summary>
        /// <param name="text">The text to be typed.</param>
        /// <param name="speed1">The delay between every character.</param>
        /// <param name="speed2">The delay between new line.</param>
        /// <param name="speed3">The delay after the new line and the delay after the [CLEARCONSOLE].</param>
        public static void Type(string text, bool debug = false, int speed1 = 90, int speed2 = 1000, int speed3 = 300)
        {
            int[] durations = { speed1, speed2, speed3 };
            if(debug)
            {
                durations[0] = 0;
                durations[1] = 0;
                durations[2] = 0;
            }
            StringBuilder builder = new StringBuilder(text);
            builder.Replace("\n\n\n", "\n[CLEARCONSOLE]");
            string updatedText = builder.ToString();
            string[] lines = updatedText.Split('\n');
            foreach (string line in lines)
            {
                if (line == "[CLEARCONSOLE]")
                {
                    Console.Clear();
                    Thread.Sleep(durations[2]);
                }
                else
                {
                    foreach (char character in line.ToCharArray())
                    {
                        Thread.Sleep(durations[0]);
                        Console.Write(character);
                    }
                }
                Thread.Sleep(durations[1]);
                Console.WriteLine();
                Thread.Sleep(durations[2]);
            }
        }

        /// <summary>
        /// Adds the variables to a text :3
        /// </summary>
        /// <param name="text">The input text UwU</param>
        /// <returns></returns>
        public static string includeVariables(string text)
        {
            string result = text;
            MatchCollection matches = Regex.Matches(text, @"\{\w+\}");

            foreach (Match matchRegex in matches)
            {
                string variable = matchRegex.Value.Substring(1, matchRegex.Value.Length - 2);
                if(File.Exists(makeSavePath("variables", variable + ".var")))
                {
                    string varData = File.ReadAllText(makeSavePath("variables", variable + ".var"));
                    result.Replace(matchRegex.Value, varData);
                }
                else
                {
                    result.Replace(matchRegex.Value, "{Variable not found}");
                }
            }

            return result;
        }

        /// <summary>
        /// Makes a path for saving the files in the game data folder
        /// </summary>
        /// <param name="directory">The directory sub-folder's name</param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string makeSavePath(string directory, string filename)
        {
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), EngineerGameData.creator));
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), EngineerGameData.creator, EngineerGameData.name));
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), EngineerGameData.creator, EngineerGameData.name, directory));
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), EngineerGameData.creator, EngineerGameData.name, directory, filename);
        }

        /// <summary>
        /// Tells the player to make a choice, but returns the choice instead of invoking a function.
        /// </summary>
        /// <param name="options">The options of the choice.</param>
        /// <returns>An integer based on the player's choice.</returns>
        public static EngineerChoiceOption ChoiceSync(EngineerChoiceOption[] options, bool isDebug)
        {
            int index = 1;
            foreach (EngineerChoiceOption option in options)
            {
                Type("[" + index.ToString() + "]: " + option.text, isDebug);
                index++;
            }
            Console.WriteLine("\nChoose one and press enter.");
            string input = Console.ReadLine();
            try
            {
                EngineerChoiceOption option = options[Int32.Parse(input) - 1];
                return option;
            }
            catch
            {
                Console.WriteLine("That... is not a valid choice, try again.");
                return ChoiceSync(options, isDebug);
            }
        }
    }
}