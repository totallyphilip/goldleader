using System;

namespace Easy
{
    public class Mathy
    {
        public static Random Random = new Random();

        public static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        public static int Round(double d)
        {
            return Convert.ToInt32(Math.Round(d, MidpointRounding.AwayFromZero));
        }

    }
}

