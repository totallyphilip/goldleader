using AsciiEngine;
using System;
using System.Collections.Generic;

namespace Easy
{
    public class Abacus
    {
        public class Slope
        {
            public double Rise;
            public double Run;
            public Slope(double rise, double run)
            {
                this.Rise = rise;
                this.Run = run;
            }
        }

        public static T RandomEnumValue<T>()
        {
            //Usage: eEnumType T = RandomEnumValue<eEnumType>();
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(new Random().Next(v.Length));
        }

        public static Random Random = new Random();
        public static bool RandomTrue { get { return Abacus.Random.NextDouble() < .5; } }
        public static double RandomDegrees { get { return Abacus.Random.Next(360); } }

        public static Slope SlopeFrom(double degrees)
        {
            // 0 degrees is east
            double radians = degrees * Math.PI / 180;
            double run = Math.Cos(radians);
            double rise = Math.Sin(radians);
            return new Slope(rise, run);
        }

        public static double Distance(AsciiEngine.Grid.Point c1, AsciiEngine.Grid.Point c2)
        {
            return Math.Sqrt(Math.Pow(c1.dX - c2.dX, 2) + Math.Pow(c1.dY - c2.dY, 2));
        }

        public static int Round(double d) { return Convert.ToInt32(Math.Round(d, MidpointRounding.AwayFromZero)); }

        public static int LesserOf(int a, int b) { if (a < b) { return a; } else { return b; } }
        public static int GreaterOf(int a, int b) { if (a > b) { return a; } else { return b; } }

        public class Fibonacci
        {
            List<int> sequence;

            public void Increment() { sequence.Insert(0, sequence[0] + sequence[1]); }

            public void Reset()
            {
                sequence = new List<int>();
                sequence.Add(1);
                sequence.Add(1);
            }

            public int Value { get { return sequence[0]; } }

            public Fibonacci() { this.Reset(); }

        }
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
            string result = s;
            for (int i = 1; i < n; i++) { result += s; }
            return result;
        }

        public static string Fluffer(string s, string fluff)
        {
            string fluffed = "";
            for (int i = 0; i < s.Length - 1; i++) { fluffed += s.ToCharArray()[i] + fluff; }
            fluffed += s.ToCharArray()[s.Length - 1];
            return fluffed;
        }

        public static string Pluralize(string s, int i)
        {
            if (i == 1) { return s + "s"; }
            else { return s; }
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
        static Int64 LastTick = 0;

        //  static Int64 StopWatchTick = 0;

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

        public class Timer
        {
            // time reported in seconds

            Int64 StartedTick;
            Int64 PausedTick = 0;
            int TimeLimit;
            bool Paused { get { return this.PausedTick > 0; } } // infer if timer is paused

            public Timer(int seconds) { this.TimeLimit = seconds; }

            public void Start() { this.StartedTick = DateTime.Now.Ticks; }

            public void Pause() { if (!this.Paused) { this.PausedTick = DateTime.Now.Ticks; } }
            public void Resume()
            {
                if (this.Paused)
                {
                    this.StartedTick += DateTime.Now.Ticks - this.PausedTick;
                    this.PausedTick = 0;
                }
            }
            public double TimeElapsed
            {
                get
                {
                    if (this.Paused) { return (this.PausedTick - this.StartedTick) / 10000000; }
                    else { return (DateTime.Now.Ticks - this.StartedTick) / 10000000; }
                }
            }

            public int TimeLeft { get { return Abacus.Round(this.TimeLeftPrecise); } }
            public double TimeLeftPrecise { get { return this.TimeLimit - this.TimeElapsed; } }

            public bool Expired { get { return this.TimeLimit - this.TimeElapsed <= 0; } }

        }
    }
}

