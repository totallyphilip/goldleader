using System;

namespace Easy
{
    public class Numbers
    {
        public static Random Random = new Random();

        public static double Distance(double x1, double x2, double y1, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        public static int Round(double d)
        {
            return Convert.ToInt32(Math.Round(d, MidpointRounding.AwayFromZero));
        }

    }

    public class Keys
    {
        public static void EatKeys()
        {
            while (Console.KeyAvailable) { Console.ReadKey(true); }
        }
    }

    public class Clock
    {
        public static Int64 LastTick = 0;

        public static void FpsThrottle(int slices)
        {
            // divide a second into slices, then wait for the fps specified
            Int64 minimumticks = 10000000 / slices;
            while (DateTime.Now.Ticks < Clock.LastTick + minimumticks) { System.Threading.Thread.Sleep(10); }

            Clock.LastTick = DateTime.Now.Ticks;
        }
    }
}

