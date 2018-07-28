using System;
using AsciiEngine;

namespace Easy
{
    public class Abacus
    {
        public static Random Random = new Random();
        public static bool RandomTrue { get { return Abacus.Random.NextDouble() < .5; } }

        public static double Distance(AsciiEngine.Grid.Point c1, AsciiEngine.Grid.Point c2)
        {
            return Math.Sqrt(Math.Pow(c1.dX - c2.dX, 2) + Math.Pow(c1.dY - c2.dY, 2));
        }

        public static int Round(double d) { return Convert.ToInt32(Math.Round(d, MidpointRounding.AwayFromZero)); }

    }

    public class Keyboard
    {
        public static void EatKeys()
        {
            while (Console.KeyAvailable) { Console.ReadKey(true); }
        }
    }

    public class Textify
    {
        public static string Repeat(string s, int n)
        {
            string result = "";
            for (int i = 0; i < n; i++) { result += s; }
            return result;
        }

        public static void WriteLineCentered(string s)
        {
            WriteCentered(s);
            Console.WriteLine();
        }
        public static void WriteCentered(string s)
        {

            Console.Write(new string(' ', Abacus.Round(Screen.Width / 2) - Abacus.Round(s.Length / 2)));
            Console.Write(s);
        }
    }

    public class Clock
    {
        public static Int64 LastTick = 0;

        public static void FpsThrottle(int slices)
        {
            // divide a second into slices, then wait for the fps specified
            Int64 minimumticks = 10000000 / slices;

            if (DateTime.Now.Ticks < Clock.LastTick + minimumticks)
            {
                System.Threading.Thread.Sleep(Convert.ToInt32(minimumticks - (DateTime.Now.Ticks - Clock.LastTick)) / 10000);
            }

            Clock.LastTick = DateTime.Now.Ticks;
        }
    }
}

