using AsciiEngine;
using Easy;
using System;
using System.Collections.Generic;

namespace AsciiEngine
{

    #region " Coordinates "

    public class Coordinate
    {


        double _X;
        double _Y;

        public double X { get { return this._X; } }
        public double Y { get { return this._Y; } }

        public void Offset(double incx, double incy)
        {
            this.Set(this._X + incx, this._Y + incy);
        }

        public void Set(double x, double y)
        {
            this._X = x;
            this._Y = y;
        }

        public Coordinate() : this(0, 0) { }
        public Coordinate(double x, double y)
        {
            this._X = x;
            this._Y = y;
        }

    }

    #endregion

    #region " Sprites "

    public class Sprite
    {

        public Coordinate XY;
        Coordinate OriginalXY;
        double _IncrementX;
        double _IncrementY;
        char _Ascii;
        double _Range;
        bool _Killed = false;

        public bool Alive
        {
            get { return Numbers.Distance(XY.X, OriginalXY.X, XY.Y, OriginalXY.Y) < this._Range && !this._Killed; }
        }

        public void Hide()
        {
            Screen.TryWrite(XY.X, XY.Y, ' ');
        }

        public void Kill()
        {
            this._Killed = true;
        }

        public void Animate()
        {
            Screen.TryWrite(XY.X, XY.Y, ' ');
            this.XY.Offset(this._IncrementX, this._IncrementY);
            Screen.TryWrite(XY.X, XY.Y, this._Ascii);
        }

        public Sprite(char c, Coordinate xy, double range) : this(c, xy, -1, -1, range) { } // random direction increments

        public Sprite(char c, Coordinate xy, double incx, double incy, double range)
        {
            this._Ascii = c;
            OriginalXY = xy;
            XY = new Coordinate(xy.X, xy.Y);

            this._Range = range;

            if (incx == -1 && incy == -1)
            {
                // add a fraction to make sure it's not zero
                this._IncrementX = Numbers.Random.NextDouble() + .1;
                this._IncrementY = Numbers.Random.NextDouble() + .1;
                if (Numbers.Random.NextDouble() < .5) { this._IncrementX *= -1; }
                if (Numbers.Random.NextDouble() < .5) { this._IncrementY *= -1; }

            }
            else
            {
                this._IncrementX = incx;
                this._IncrementY = incy;
            }

        }

    }

    public class SpriteField
    {

        List<Sprite> _sprites = new List<Sprite>();

        public List<Sprite> Sprites
        {
            get { return _sprites; }
        }

        public void RemoveSprite(Sprite s)
        {
            this.Sprites.Find(x => s.Equals(x)).Kill();
        }

        public void Animate()
        {

            foreach (Sprite s in this._sprites.FindAll(x => !x.Alive))
            {
                s.Hide();
                this.Sprites.Remove(s);
            }

            foreach (Sprite s in this._sprites.FindAll(x => x.Alive)) { s.Animate(); }
        }

        public SpriteField() { }

    }

    #endregion

    #region " Screen "

    public class Screen
    {

        #region " Dimensions "

        public static int TopEdge { get { return 0; } }

        public static int BottomEdge { get { return Console.WindowHeight - 1; } }

        public static int LeftEdge { get { return 0; } }

        public static int RightEdge { get { return Console.WindowWidth - 1; } }

        public static int Width { get { return Console.WindowWidth; } }

        public static int Height { get { return Console.WindowHeight; } }

        public static Coordinate GetCenterCoordinate()
        {
            return new Coordinate(Numbers.Round(Screen.Width / 2), Numbers.Round(Screen.Height / 2));
        }

        public static bool TrySetSize(int targetwidth, int targetheight)
        {
            return TrySetSize(targetwidth, targetheight, true);
        }

        public static bool TrySetSize(int targetwidth, int targetheight, bool adjustmanually)
        {

            try
            {
                Console.SetWindowSize(targetwidth, targetheight);
                Console.SetBufferSize(targetwidth, targetheight);
            }
            catch
            {

                while ((Console.WindowWidth != targetwidth || Console.WindowHeight != targetheight || !Console.KeyAvailable) && !Console.KeyAvailable && adjustmanually)
                {

                    Console.Clear();

                    if (Console.WindowWidth == targetwidth && Console.WindowHeight == targetheight)
                    {
                        Console.Write("Perfect size!\nPress ENTER to continue.");
                    }
                    else
                    {
                        Console.WriteLine("Resize windown to");
                        Console.WriteLine(targetwidth + " X " + targetheight);
                        Console.WriteLine("or keypress to quit.");
                        Console.WriteLine();

                        char leftarrow = '<'; // \x2190
                        char rightarrow = '>'; // \x2192
                        char uparrow = '^'; // \x2191
                        char downarrow = 'v'; // \x2193

                        Console.WriteLine(Console.WindowWidth + " X " + Console.WindowHeight);

                        Console.Write("resize: " + Console.WindowWidth + " ");
                        if (Console.WindowWidth > targetwidth) { Console.WriteLine(new String(leftarrow, Console.WindowWidth - targetwidth)); }
                        else if (Console.WindowWidth < targetwidth) { Console.WriteLine(new String(rightarrow, targetwidth - Console.WindowWidth)); }
                        else { Console.WriteLine("Perfect!"); }

                        Console.Write("resize: " + Console.WindowHeight + " ");
                        if (Console.WindowHeight > targetheight) { Console.Write(new String(uparrow, Console.WindowHeight - targetheight)); }
                        else if (Console.WindowHeight < targetheight) { Console.Write(new String(downarrow, targetheight - Console.WindowHeight)); }
                        else { Console.Write("Perfect!"); }
                    }

                    System.Threading.Thread.Sleep(200); // good enough so the CPU doesn't go crazy

                }

            }

            return (Console.WindowHeight == targetheight && Console.WindowWidth == targetwidth);

        }

        #endregion

        #region " Writing "

        public static void TryWrite(Coordinate xy, string s)
        {
            TryWrite(xy.X, xy.Y, s);
        }
        public static void TryWrite(double x, double y, string s)
        {
            TryWrite(Numbers.Round(x), Numbers.Round(y), s);
        }
        public static void TryWrite(int x, int y, string s)
        {
            try
            {
                if (y >= Screen.TopEdge && y <= Screen.BottomEdge)
                {
                    // the whole string should fit on the screen
                    if (x >= Screen.LeftEdge && x + s.Length < Screen.RightEdge)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(s);
                    }
                    // some or all of the text is off the screen, so go character by character
                    else
                    {
                        char[] chars = s.ToCharArray();
                        for (int c = 0; c < chars.Length; c++) { Screen.TryWrite(x + c, y, chars[c]); }
                    }
                }
            }
            catch { }
        }

        public static void TryWrite(Coordinate xy, char c)
        {
            TryWrite(xy.X, xy.Y, c);
        }

        public static void TryWrite(double x, double y, char c)
        {
            TryWrite(Numbers.Round(x), Numbers.Round(y), c);
        }

        public static void TryWrite(int x, int y, char c)
        {
            // don't write anything past the screen edges
            // don't write anything in the lower right corner because that can cause scrolling
            if (x >= Screen.LeftEdge && x <= Screen.RightEdge && y >= Screen.TopEdge && y <= Screen.BottomEdge && !(x == Screen.RightEdge && y == Screen.BottomEdge))
            {
                Console.SetCursorPosition(x, y);
                Console.Write(c);
            }
        }

        public static void Countdown(int start)
        {
            Keys.EatKeys();
            Coordinate xy = Screen.GetCenterCoordinate();

            for (int n = start; n > 0; n--)
            {
                Screen.TryWrite(xy.X, xy.Y, n + " ");
                if (Console.KeyAvailable) { n = 0; }
                else { System.Threading.Thread.Sleep(1000); }
            }
        }

        #endregion
    }

    #endregion 

}
