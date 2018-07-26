using System;
using System.Collections.Generic;

namespace AsciiEngine
{
    namespace Fx
    {
        public class Explosion : Sprites.Swarm
        {
            public Explosion(char[] ascii, Coordinates.Point coord, double range, bool centered, bool up, bool down, bool left, bool right)
            {
                for (int i = 0; i < ascii.Length; i++)
                {
                    double rise = Easy.Abacus.Random.NextDouble() + .01; // right (at least slightly)
                    double run = Easy.Abacus.Random.NextDouble() + .01; // down (at least slightly)

                    if (!up && !down) { rise = 0; } // no rise
                    if (up && !down) { rise *= -1; } // go up
                    if (up && down) { if (Easy.Abacus.RandomTrue) { rise *= -1; } } // surprise me

                    if (!left && !right) { run = 0; } // no run
                    if (left && !right) { run *= -1; } // go left
                    if (left && right) { if (Easy.Abacus.RandomTrue) { run *= -1; } } // surprise me

                    Coordinates.Trajectory t = new Coordinates.Trajectory(rise, run, range);
                    Coordinates.Point xy = new Coordinates.Point(coord.dY, coord.dY);

                    if (centered) { xy.dX = coord.dX + ascii.Length / 2; } else { xy.dX += i; }

                    this.Items.Add(new Sprites.Sprite(new[] { ascii[i] }, xy, t));
                }

            }

        }

    }

    namespace Sprites
    {
        public class Sprite
        {

            #region " Fly Zone "

            public class FlyZoneClass

            {
                public enum eEdgeMode { Ignore, Bounce, Stop }
                public eEdgeMode EdgeMode = eEdgeMode.Ignore;

                public int TopMargin = 0;
                public int BottomMargin = 0;
                public int LeftMargin = 0;
                public int RightMargin = 0;

                public int TopEdge { get { return Screen.TopEdge + this.TopMargin; } }
                public int BottomEdge { get { return Screen.BottomEdge - this.BottomMargin; } }
                public int LeftEdge { get { return Screen.LeftEdge + this.LeftMargin; } }
                public int RightEdge { get { return Screen.RightEdge - this.RightMargin; } }
                public int Width { get { return this.RightEdge - this.LeftEdge + 1; } }



                public FlyZoneClass(int topmargin, int bottommargin, int leftmargin, int rightmargin) : this(topmargin, bottommargin, leftmargin, rightmargin, eEdgeMode.Ignore) { }

                public FlyZoneClass(int topmargin, int bottommargin, int leftmargin, int rightmargin, eEdgeMode edgemode)
                {
                    this.TopMargin = topmargin;
                    this.BottomMargin = bottommargin;
                    this.LeftMargin = leftmargin;
                    this.RightMargin = rightmargin;
                    this.EdgeMode = edgemode;
                }
            }

            protected FlyZoneClass FlyZone = new FlyZoneClass(0, 0, 0, 0);

            #endregion

            #region " Locations "

            public Coordinates.Trail Trail;
            public Coordinates.Trajectory Trajectory;
            public Coordinates.Point XY { get { return this.Trail.XY; } }

            #endregion

            #region " Status "

            public int HP = int.MaxValue;
            public bool Active = true;
            bool Terminated = false;
            protected int Width { get { return this.Ascii.Length; } }

            public bool Alive
            {
                get
                {
                    bool alive = !this.Terminated && this.HP > 0;
                    if (alive && Trajectory != null)
                    {
                        alive = Easy.Abacus.Distance(this.Trail.XY, this.Trail.InitialXY) < Trajectory.Range;
                    }
                    return alive && AliveOverride;
                }
            }

            protected virtual bool AliveOverride { get { return true; } } // optional additional code in overriding property

            public bool Hit(Coordinates.Point coord) { return this.Hit(coord, -1); } // default 1 damage
            public bool Hit(Coordinates.Point coord, int HealthEffect)
            {
                // Hits are only detected if:
                // * the sprite is still alive
                // * the sprite is active (i.e. still doing something else even if dead)
                // * the given coordinate is within the sprite body
                bool HitDetected = this.Alive && this.Active && coord.dX >= this.XY.dX && coord.dX < this.XY.dX + this.Width && Easy.Abacus.Round(coord.dY) == Easy.Abacus.Round(this.XY.dY);
                if (HitDetected) { this.HP += HealthEffect; }
                return HitDetected;
            }

            public void Terminate() { this.Terminated = true; }

            #endregion

            #region " Animation "

            public char[] Ascii;

            public void Hide()
            {
                Screen.TryWrite(this.XY, new String(' ', this.Width));
            }

            public void Animate()
            {
                this.Hide();
                this.Trail.Items.Add(this.NextCoordinate());
                Screen.TryWrite(this.XY, new String(this.Ascii));
            }

            protected virtual Coordinates.Point NextCoordinate()
            {
                if (this.XY.dY + this.Trajectory.Rise < this.FlyZone.TopEdge)
                {
                    if (this.FlyZone.EdgeMode == FlyZoneClass.eEdgeMode.Bounce) { this.Trajectory.Rise = Math.Abs(this.Trajectory.Rise); }
                    else if (this.FlyZone.EdgeMode == FlyZoneClass.eEdgeMode.Stop) { this.Trajectory.Rise = 0; }
                    else if (this.FlyZone.EdgeMode == FlyZoneClass.eEdgeMode.Ignore) { } // else-if not needed, just here for clarity
                }
                else if (this.XY.dY + this.Trajectory.Rise > this.FlyZone.BottomEdge)
                {
                    if (this.FlyZone.EdgeMode == FlyZoneClass.eEdgeMode.Bounce) { this.Trajectory.Rise = Math.Abs(this.Trajectory.Rise) * (-1); }
                    else if (this.FlyZone.EdgeMode == FlyZoneClass.eEdgeMode.Stop) { this.Trajectory.Rise = 0; }
                    else if (this.FlyZone.EdgeMode == FlyZoneClass.eEdgeMode.Ignore) { } // else-if not needed, just here for clarity
                }

                if (this.XY.dX + this.Trajectory.Run < this.FlyZone.LeftEdge)
                {
                    if (this.FlyZone.EdgeMode == FlyZoneClass.eEdgeMode.Bounce) { this.Trajectory.Run = Math.Abs(this.Trajectory.Run); }
                    else if (this.FlyZone.EdgeMode == FlyZoneClass.eEdgeMode.Stop) { this.Trajectory.Run = 0; }
                    else if (this.FlyZone.EdgeMode == FlyZoneClass.eEdgeMode.Ignore) { } // else-if not needed, just here for clarity
                }

                if (this.XY.dX + this.Trajectory.Run + this.Width > this.FlyZone.RightEdge)
                {
                    if (this.FlyZone.EdgeMode == FlyZoneClass.eEdgeMode.Bounce) { this.Trajectory.Run = Math.Abs(this.Trajectory.Run) * (-1); }
                    else if (this.FlyZone.EdgeMode == FlyZoneClass.eEdgeMode.Stop) { this.Trajectory.Run = 0; }
                    else if (this.FlyZone.EdgeMode == FlyZoneClass.eEdgeMode.Ignore) { } // else-if not needed, just here for clarity
                }

                return new Coordinates.Point(this.Trail.XY.dX + this.Trajectory.Run, this.Trail.XY.dY + this.Trajectory.Rise);

            }

            public virtual void DoActivities() // add more complex code in inherited tasks if needed
            {
                if (this.Alive)
                {
                    // stuff to do while alive
                }

                // stuff to do regardless

                if (!this.Alive) { this.Active = false; } // set false when no more stuff to do
            }


            #endregion

            #region  " Constructor "

            public Sprite() { }

            public Sprite(Coordinates.Point xy, Coordinates.Trajectory t)
            {
                this.Ascii = "sprite".ToCharArray();
                this.Trail = new Coordinates.Trail(xy);
                this.Trajectory = t;
            }

            public Sprite(char[] c, Coordinates.Point xy, Coordinates.Trajectory t)
            {
                this.Ascii = new List<char>(c).ToArray();
                this.Trail = new Coordinates.Trail(xy);
                this.Trajectory = t;
            }

            #endregion

        }

        public class Swarm
        {

            public List<Sprite> Items = new List<Sprite>();

            public void Animate()
            {

                foreach (Sprite s in this.Items.FindAll(x => !x.Alive && !x.Active))
                {
                    s.Hide();
                    this.Items.Remove(s);
                }

                foreach (Sprite s in this.Items.FindAll(x => x.Alive)) { s.Animate(); }
                foreach (Sprite s in this.Items.FindAll(x => x.Active)) { s.DoActivities(); }

                this.Spawn();

            }

            protected virtual void Spawn() { } // do nothing unless inherited class overrides this

            public Swarm() { }

        }

        #region " Specialty Swarms "
        #endregion
    }

    namespace Coordinates
    {

        public class Point
        {

            public double dX;
            public double dY;
            public int iX { get { return Easy.Abacus.Round(this.dX); } }
            public int iY { get { return Easy.Abacus.Round(this.dY); } }

            public Point() : this(0, 0) { }
            public Point(double dx, double dy)
            {
                this.dX = dx;
                this.dY = dy;
            }

        }

        public class Trail
        {

            public List<Point> Items = new List<Point>();

            public Point XY { get { return Items[Items.Count - 1]; } } // current XY
            public Point PreviousXY { get { return Items[Items.Count - 2]; } }

            public Point InitialXY { get { return Items[0]; } }

            public void MoveTo(Point xy)
            {
                Items.Add(xy);
            }

            public Trail(Point xy)
            {
                Items.Add(xy);
            }
        }

        public class Trajectory
        {
            double _Run;
            double _Rise;
            double _Range;

            public double Run { get { return _Run; } set { _Run = value; } }
            public double Rise { get { return _Rise; } set { _Rise = value; } }
            public double Range { get { return _Range; } set { _Range = value; } }

            #region " Constructor "

            public Trajectory(double rise, double run, double range)
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
                this._Run = Easy.Abacus.Random.NextDouble() + .1;
                this._Rise = Easy.Abacus.Random.NextDouble() + .1;
                if (Easy.Abacus.RandomTrue) { this._Run *= -1; }
                if (Easy.Abacus.RandomTrue) { this._Rise *= -1; }
            }

            #endregion
        }
    }
    public class Screen
    {

        #region " Dimensions "

        public static int TopEdge { get { return 0; } }

        public static int BottomEdge { get { return Console.WindowHeight - 1; } }

        public static int LeftEdge { get { return 0; } }

        public static int RightEdge { get { return Console.WindowWidth - 1; } }

        public static int Width { get { return Console.WindowWidth; } }

        public static int Height { get { return Console.WindowHeight; } }

        public static Coordinates.Point GetCenterCoordinate()
        {
            return new Coordinates.Point(Easy.Abacus.Round(Screen.Width / 2), Easy.Abacus.Round(Screen.Height / 2));
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

        public static void TryWrite(Coordinates.Point xy, char[] s)
        {
            TryWrite(xy.dX, xy.dY, s.ToString());
        }

        public static void TryWrite(Coordinates.Point xy, string s)
        {
            TryWrite(xy.dX, xy.dY, s);
        }
        public static void TryWrite(double x, double y, string s)
        {
            TryWrite(Easy.Abacus.Round(x), Easy.Abacus.Round(y), s);
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

        public static void TryWrite(Coordinates.Point xy, char c)
        {
            TryWrite(xy.dX, xy.dY, c);
        }

        public static void TryWrite(double x, double y, char c)
        {
            TryWrite(Easy.Abacus.Round(x), Easy.Abacus.Round(y), c);
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
            Easy.Keyboard.EatKeys();
            Coordinates.Point xy = Screen.GetCenterCoordinate();

            for (int n = start; n > 0; n--)
            {
                Screen.TryWrite(xy.dX, xy.dY, n + " ");
                if (Console.KeyAvailable) { n = 0; }
                else { System.Threading.Thread.Sleep(1000); }
            }
        }

        #endregion
    }

}
