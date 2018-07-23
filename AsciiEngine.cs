using AsciiEngine;
using System;
using System.Collections.Generic;

namespace AsciiEngine
{
    #region " Sprites "

    public class Sprite
    {

        #region " Locations "

        public Screen.CoordinateHistory Trail;
        public Screen.Trajectory Trajectory;
        public Screen.Coordinate XY { get { return this.Trail.XY; } }

        #endregion

        #region " Status "

        bool Terminated = false;

        public bool Alive
        {
            get
            {
                bool alive = !this.Terminated;
                if (alive && Trajectory != null)
                {
                    alive = Easy.Numbers.Distance(this.Trail.XY, this.Trail.InitialXY) < Trajectory.Range;
                }
                return alive;
            }
        }

        public void Terminate()
        {
            this.Terminated = true;
        }

        #endregion

        #region " Animation "

        protected char[] Ascii;

        public void Hide()
        {
            Screen.TryWrite(this.XY, new String(' ', this.Ascii.Length));
        }

        public void Animate()
        {
            this.Hide();
            this.Trail.Items.Add(new Screen.Coordinate(this.Trail.XY.X + this.Trajectory.Run, this.Trail.XY.Y + this.Trajectory.Rise));
            Screen.TryWrite(this.XY, new String(this.Ascii));
        }

        #endregion

        #region  " Constructor "

        public Sprite(Screen.Coordinate xy, Screen.Trajectory t)
        {
            this.Ascii = "sprite".ToCharArray();
            this.Trail = new Screen.CoordinateHistory(xy);

            Trail = new Screen.CoordinateHistory(xy);
            this.Trajectory = t;
        }

        public Sprite(char[] c, Screen.Coordinate xy, Screen.Trajectory t)
        {
            this.Ascii = new List<char>(c).ToArray();
            this.Trail = new Screen.CoordinateHistory(xy);

            Trail = new Screen.CoordinateHistory(xy);
            this.Trajectory = t;
        }

        #endregion

    }

    public class SpriteField
    {

        public List<Sprite> Items = new List<Sprite>();

        public void AddSprite(Sprite s)
        {
            this.Items.Add(s);
        }

        public void RemoveSprite(Sprite s)
        {
            this.Items.Find(x => s.Equals(x)).Terminate();
        }

        public void Animate()
        {

            foreach (Sprite s in this.Items.FindAll(x => !x.Alive))
            {
                s.Hide();
                this.Items.Remove(s);
            }

            foreach (Sprite s in this.Items.FindAll(x => x.Alive)) { s.Animate(); }

            AnimateOverride();

        }

        protected virtual void AnimateOverride() { } // optional additional code in overriding method

        public SpriteField() { }

    }

    #endregion

    #region " Screen "

    public class Screen
    {
        #region " Coordinates "

        public class Coordinate
        {

            public double X;
            public double Y;

            public Coordinate() : this(0, 0) { }
            public Coordinate(double x, double y)
            {
                this.X = x;
                this.Y = y;
            }

        }

        public class CoordinateHistory
        {

            public List<Coordinate> Items = new List<Coordinate>();

            public Coordinate XY { get { return Items[Items.Count - 1]; } } // current XY
            public Coordinate PreviousXY { get { return Items[Items.Count - 2]; } }

            public Coordinate InitialXY { get { return Items[0]; } }

            public void MoveTo(Coordinate xy)
            {
                Items.Add(xy);
            }

            public CoordinateHistory(Coordinate xy)
            {
                Items.Add(xy);
            }
        }

        #endregion

        #region " Trajectory "

        public class Trajectory
        {
            double _Run;
            double _Rise;
            double _Range;

            public double Run { get { return _Run; } set { _Run = value; } }
            public double Rise { get { return _Rise; } set { _Rise = value; } }
            public double Range { get { return _Range; } set { _Range = value; } }

            #region " Constructor "

            public Trajectory(double run, double rise, double range)
            {
                this._Rise = rise;
                this._Run = run;
                this._Range = range;
            }

            public Trajectory() : this(double.MaxValue) { }
            public Trajectory(double rise, double run) : this(rise, run, double.MaxValue) { }

            public Trajectory(double range)
            {
                this._Range = range;
                // add a fraction to make sure it's not zero
                this._Run = Easy.Numbers.Random.NextDouble() + .1;
                this._Rise = Easy.Numbers.Random.NextDouble() + .1;
                if (Easy.Numbers.Random.NextDouble() < .5) { this._Run *= -1; }
                if (Easy.Numbers.Random.NextDouble() < .5) { this._Rise *= -1; }
            }

            #endregion
        }


        #endregion

        #region " Dimensions "

        public static int TopEdge { get { return 0; } }

        public static int BottomEdge { get { return Console.WindowHeight - 1; } }

        public static int LeftEdge { get { return 0; } }

        public static int RightEdge { get { return Console.WindowWidth - 1; } }

        public static int Width { get { return Console.WindowWidth; } }

        public static int Height { get { return Console.WindowHeight; } }

        public static Screen.Coordinate GetCenterCoordinate()
        {
            return new Screen.Coordinate(Easy.Numbers.Round(Screen.Width / 2), Easy.Numbers.Round(Screen.Height / 2));
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

        public static void TryWrite(Screen.Coordinate xy, string s)
        {
            TryWrite(xy.X, xy.Y, s);
        }
        public static void TryWrite(double x, double y, string s)
        {
            TryWrite(Easy.Numbers.Round(x), Easy.Numbers.Round(y), s);
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

        public static void TryWrite(Screen.Coordinate xy, char c)
        {
            TryWrite(xy.X, xy.Y, c);
        }

        public static void TryWrite(double x, double y, char c)
        {
            TryWrite(Easy.Numbers.Round(x), Easy.Numbers.Round(y), c);
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
            Easy.Keys.EatKeys();
            Screen.Coordinate xy = Screen.GetCenterCoordinate();

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
