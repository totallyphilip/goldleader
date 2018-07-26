using System;

namespace Easy
{
    public class Numbers
    {
        public static Random Random = new Random();

        public static double Distance(AsciiEngine.Coordinates.Coordinate c1, AsciiEngine.Coordinates.Coordinate c2)
        {
            return Math.Sqrt(Math.Pow(c1.X - c2.X, 2) + Math.Pow(c1.Y - c2.Y, 2));
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

            if (DateTime.Now.Ticks < Clock.LastTick + minimumticks)
            {
                System.Threading.Thread.Sleep(Convert.ToInt32(minimumticks - (DateTime.Now.Ticks - Clock.LastTick)) / 10000);
            }

            Clock.LastTick = DateTime.Now.Ticks;
        }
    }
}

