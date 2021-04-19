using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EngineerLibraries
{
    class EngineerLibraries
    {
        /// <summary>
        /// Used to animate a typing style.
        /// </summary>
        /// <param name="text">The text to be typed.</param>
        /// <param name="speed1">The delay between every character.</param>
        /// <param name="speed2">The delay between new line.</param>
        /// <param name="speed3">The delay after the new line and the delay after the [CLEARCONSOLE].</param>
        public static void Type(string text, int speed1 = 90, int speed2 = 1000, int speed3 = 300)
        {
            int[] durations = { speed1, speed2, speed3 };
            StringBuilder builder = new StringBuilder(text);
            builder.Replace("\n\n\n", "\n\n[CLEARCONSOLE]");
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
        /// Tells the player to make a choice, but returns the choice instead of invoking a function.
        /// </summary>
        /// <param name="options">The options of the choice.</param>
        /// <returns>An integer based on the player's choice.</returns>
        public static int ChoiceSync(string[] options)
        {
            int index = 1;
            foreach (string option in options)
            {
                Type("[" + index.ToString() + "]: " + option);
                index++;
            }
            Console.WriteLine("\nChoose one and press enter.");
            string input = Console.ReadLine();
            try
            {
                string bruh = options[Int32.Parse(input) - 1];
                return Int32.Parse(input);
            }
            catch
            {
                Console.WriteLine("That... is not a valid choice.");
                return ChoiceSync(options);
            }
        }
    }
}
