using System;
using System.Collections.Generic;
public class AsciiEngine
{

    #region " Sprites "

    public class Sprite
    {

        double _OriginalX;
        double _OriginalY;
        double _X;
        double _Y;
        double _IncrementX;
        double _IncrementY;
        char _Ascii;
        double _Range;
        bool _Killed = false;

        public int X { get { return AsciiEngine.RoundNumber(this._X); } }
        public int Y { get { return AsciiEngine.RoundNumber(this._Y); } }

        public bool Alive
        {
            get { return Math.Sqrt(Math.Pow(this._X - this._OriginalX, 2) + Math.Pow(this._Y - this._OriginalY, 2)) < this._Range && !this._Killed; }
        }

        public void Hide()
        {
            AsciiEngine.TryWrite(AsciiEngine.RoundNumber(this._X), AsciiEngine.RoundNumber(this._Y), ' ');
        }

        public void Kill()
        {
            this._Killed = true;
        }
        public void Animate()
        {
            this.Hide();
            this._X += this._IncrementX;
            this._Y += this._IncrementY;
            AsciiEngine.TryWrite(AsciiEngine.RoundNumber(this._X), AsciiEngine.RoundNumber(this._Y), this._Ascii);
        }

        public Sprite(char c, double x, double y, double range) : this(c, x, y, -1, -1, range) { } // random direction increments

        public Sprite(char c, double x, double y, double incx, double incy, double range)
        {
            this._Ascii = c;
            this._OriginalX = x;
            this._OriginalY = y;
            this._X = x;
            this._Y = y;
            this._Range = range;

            if (incx == -1 && incy == -1)
            {
                Random r = new Random();
                // add a fraction to make sure it's not zero
                this._IncrementX = r.NextDouble() + .1;
                this._IncrementY = r.NextDouble() + .1;
                if (r.NextDouble() < .5) { this._IncrementX *= -1; }
                if (r.NextDouble() < .5) { this._IncrementY *= -1; }

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

    #region " Static Properties "

    public static int TopEdge { get { return 0; } }

    public static int BottomEdge { get { return Console.WindowHeight - 1; } }

    public static int LeftEdge { get { return 0; } }

    public static int RightEdge { get { return Console.WindowWidth - 1; } }

    public static int Width { get { return Console.WindowWidth; } }

    public static int Height { get { return Console.WindowHeight; } }

    #endregion

    #region " Screen Manipulation "

    public static int RoundNumber(double d)
    {
        return Convert.ToInt32(Math.Round(d, MidpointRounding.AwayFromZero));
    }

    public static bool TrySetWindowSize(int setwidth, int setheight)
    {
        return TrySetWindowSize(setwidth, setheight, true);
    }

    public static bool TrySetWindowSize(int setwidth, int setheight, bool manualadjust)
    {

        try
        {
            Console.SetWindowSize(setwidth, setheight);
            Console.SetBufferSize(setwidth + 100, setheight + 100);
        }
        catch
        {

            while ((Console.WindowWidth != setwidth || Console.WindowHeight != setheight) && (!Console.KeyAvailable) && manualadjust)
            {

                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("can't resize window.");
                Console.WriteLine("please adjust manually.");
                Console.WriteLine();

                Console.Write("adjust width: ");
                if (Console.WindowWidth > setwidth) { Console.WriteLine(new String('\x2190', Console.WindowWidth - setwidth)); }
                else if (Console.WindowWidth < setwidth) { Console.WriteLine(new String('\x2192', setwidth - Console.WindowWidth)); }
                else { Console.WriteLine("Perfect!"); }

                Console.Write("adjust height: ");
                if (Console.WindowHeight > setheight) { Console.Write(new String('\x2191', Console.WindowHeight - setheight)); }
                else if (Console.WindowHeight < setheight) { Console.Write(new String('\x2193', setheight - Console.WindowHeight)); }
                else { Console.Write("Perfect!"); }

                System.Threading.Thread.Sleep(100); // good enough so the CPU doesn't go crazy

            }

        }

        return (Console.WindowHeight == setheight && Console.WindowWidth == setwidth);

    }

    #endregion

    public static char CharPrompt(string s)
    {
        Console.Write(s);
        return Console.ReadKey(true).KeyChar;
    }

    public static bool TryWrite(int x, int y, string s)
    {
        if (y >= AsciiEngine.TopEdge && y <= AsciiEngine.BottomEdge)
        {
            if (x >= AsciiEngine.LeftEdge && x + s.Length < AsciiEngine.RightEdge)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(s);
                return true;
            }
            else
            {
                char[] chars = s.ToCharArray();
                for (int c = 0; c < chars.Length; c++) { AsciiEngine.TryWrite(x + c, y, chars[c]); }
                return (x >= AsciiEngine.LeftEdge && x + s.Length - 1 <= AsciiEngine.RightEdge && y >= AsciiEngine.TopEdge && y <= AsciiEngine.BottomEdge);
            }
        }
        else
        { return false; }
    }

    public static bool TryWrite(int x, int y, char c)
    {
        // don't write anything past the screen edges, nor in lower right corner
        if (x >= AsciiEngine.LeftEdge && x <= AsciiEngine.RightEdge && y >= AsciiEngine.TopEdge && y <= AsciiEngine.BottomEdge && !(x == AsciiEngine.RightEdge && y == AsciiEngine.BottomEdge))
        {
            Console.SetCursorPosition(x, y);
            Console.Write(c);
            return true;
        }
        else { return false; }
    }

}
