using System;
using System.Collections.Generic;
using System.Linq;

namespace AsciiEngine
{
    namespace Fx
    {
        public class Explosion : Sprites.Swarm
        {

            public Explosion() { }

            public Explosion(char[] ascii, Grid.Point coord, int width, double range, double force, bool up, bool down, bool left, bool right)
            {
                constructor(ascii, coord, width, range, force, up, down, left, right);
            }

            void constructor(char[] ascii, Grid.Point coord, int width, double range, double force, bool up, bool down, bool left, bool right)
            {

                int position = 0;

                foreach (char c in ascii)
                {
                    double rise = force * (Easy.Abacus.Random.NextDouble() + .1); // right (at least slightly)
                    double run = force * (Easy.Abacus.Random.NextDouble() + .1); // down (at least slightly)

                    if (!up && !down) { rise = 0; } // no rise
                    if (up && !down) { rise *= -1; } // go up
                    if (up && down) { if (Easy.Abacus.RandomTrue) { rise *= -1; } } // surprise me

                    if (!left && !right) { run = 0; } // no run
                    if (left && !right) { run *= -1; } // go left
                    if (left && right) { if (Easy.Abacus.RandomTrue) { run *= -1; } } // surprise me

                    Grid.Trajectory t = new Grid.Trajectory(rise, run, range);
                    Grid.Point xy = coord.Clone();

                    xy.dX += position;
                    position++;
                    if (position > width) { position = 0; }

                    this.Items.Add(new Sprites.Sprite(new[] { c }, xy, t));
                }

            }

        }

        public class Scroller : Sprites.Swarm
        {

            public int Range = 0;
            public int LineSpacing = 1;
            public double Speed = 1;
            ConsoleColor HighColor;
            ConsoleColor LowColor;

            public void NewLine(int lines) { for (int i = 0; i < lines; i++) { this.NewLine(); } }
            public void NewLine() { NewLine(""); }

            public void NewLine(string s)
            {
                int y = Screen.Height;

                int range = Screen.Height; // default range

                if (this.Range > 0) { range = this.Range; }

                foreach (Sprites.Sprite message in Items)
                {
                    if (message.XY.iY >= y) { y = message.XY.iY + this.LineSpacing; }
                }

                this.Items.Add(new Sprites.Sprite(s.ToCharArray(), new Grid.Point(Screen.Width / 2 - s.Length / 2, y), new Grid.Trajectory(-1 * this.Speed, 0, range + (y - Screen.Height)), this.HighColor));

            }

            protected override void OnAnimated()
            {
                foreach (Sprites.Sprite s in this.Items)
                {
                    if (Easy.Abacus.Distance(s.Trail.XY, s.Trail.InitialXY) > s.Trajectory.Range * .90)
                    {
                        s.Color = this.LowColor;
                    }
                }
            }

            public Scroller(int spacing) { this.constructor(spacing, 0, 1, ConsoleColor.Gray, ConsoleColor.DarkGray); }
            public Scroller(int spacing, int distance) { this.constructor(spacing, distance, 1, ConsoleColor.Gray, ConsoleColor.DarkGray); }
            public Scroller(int spacing, int distance, double speed) { this.constructor(spacing, distance, speed, ConsoleColor.Gray, ConsoleColor.DarkGray); }
            public Scroller(int spacing, int distance, double speed, ConsoleColor hicolor, ConsoleColor locolor) { this.constructor(spacing, distance, speed, hicolor, locolor); }

            public void constructor(int spacing, int distance, double speed, ConsoleColor hicolor, ConsoleColor locolor)
            {
                this.LineSpacing = spacing;
                this.Range = distance;
                this.Speed = speed;
                this.HighColor = hicolor;
                this.LowColor = locolor;
            }

        }

    }

    namespace Sprites
    {
        public class Sprite // an independently operating entity
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

            public Grid.Trail Trail;
            public Grid.Trajectory Trajectory;

            public Grid.Trajectory OriginalTrajectory;
            public Grid.Point XY { get { return this.Trail.XY; } }

            #endregion

            #region " Status "

            public int HitPoints = int.MaxValue;
            public int HitEffect = -1;
            protected int Score = 0;
            public bool Active = true;
            public bool Shown = true;
            public ConsoleColor Color = Console.ForegroundColor;
            bool Terminated = false;
            public int Width { get { return this.Ascii.Length; } }

            public int CollectScore()
            {
                int score = this.Score;
                this.Score = 0;
                return score;
            }

            public bool Alive
            {
                get
                {
                    bool alive = !this.Terminated && this.HitPoints > 0;
                    if (alive && Trajectory != null)
                    {
                        alive = Easy.Abacus.Distance(this.Trail.XY, this.Trail.InitialXY) < Trajectory.Range;
                    }
                    if (!alive && AliveOverride) { this.Hide(); }
                    return alive && AliveOverride;
                }
            }

            protected virtual bool AliveOverride { get { return true; } } // optional additional code in overriding property

            public static bool Collided(Sprite s1, Sprite s2)
            {
                bool hit = false;
                if (s1.Hit(s2)) { hit = true; s2.OnHit(s1.HitEffect); }
                else { if (s2.Hit(s1)) { hit = true; s1.OnHit(s2.HitEffect); } }
                return hit;
            }

            public bool Hit(Sprite thatone)
            {
                // Hits are only detected if all conditions met:
                // * the sprite is still alive
                // * the sprite is active (i.e. still doing something else even if dead)
                // * the given coordinate is within the sprite body

                bool HitDetected =
                    (this.Alive && this.Active)
                    && (thatone.Alive && thatone.Active)
                    && thatone.XY.iY == this.XY.iY
                    && (
                        (thatone.XY.iX >= this.XY.iX && thatone.XY.iX < this.XY.iX + this.Width) // [(]
                        || (thatone.XY.iX + thatone.Width >= this.XY.iX && thatone.XY.iX + thatone.Width < this.XY.iX + this.Width) // [)]
                        || (thatone.XY.iX <= this.XY.iX && thatone.XY.iX + thatone.Width > this.XY.iX + this.Width) // ([])
                    );

                if (HitDetected) { OnHit(thatone.HitEffect); }

                return HitDetected;
            }

            public virtual void OnHit(int hiteffect) { this.HitPoints += hiteffect; }

            public void Terminate()
            {
                this.Terminated = true;
                this.Hide();
            }

            #endregion

            #region " Squadron "

            public Sprite Leader;
            public bool HasLeader { get { return this.Leader != null; } }
            public bool LeaderEquals(Sprite s) { return this.HasLeader && this.Leader.Equals(s); }
            public void GoRogue()
            {
                this.Leader = null;
                this.Trajectory = this.OriginalTrajectory;
            }

            #endregion

            #region " Animation "

            public char[] Ascii;

            public void Hide()
            {
                if (this.Shown)
                {
                    Screen.TryWrite(this.XY, new String(' ', this.Width));
                    this.Shown = false;
                }
            }
            public void Refresh()
            {
                ConsoleColor savecolor = Console.ForegroundColor;
                Console.ForegroundColor = this.Color;
                Screen.TryWrite(this.XY, new String(this.Ascii));
                this.Shown = true;
                Console.ForegroundColor = savecolor;
            }

            public void Animate() { this.Animate(true); }

            public void Animate(bool hide)
            {
                if (hide) { this.Hide(); }
                this.Trail.Add(this.NextCoordinate());
                this.Refresh();
            }

            public virtual void Activate() // add more complex code in inherited tasks if needed
            {
                if (this.Alive)
                {
                    // stuff to do if alive
                }

                // stuff to do regardless

                if (!this.Alive) { this.Active = false; } // set false when no more stuff to do
            }

            protected virtual Grid.Point NextCoordinate()
            {

                // only worry about the fly zone if this sprite has no leader

                if (this.HasLeader)
                {
                    this.Trajectory = Leader.Trajectory.Clone();
                }
                else
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
                }

                return this.Trail.XY.Clone(this.Trajectory);
            }

            #endregion

            #region  " Constructor "

            public Sprite() { }

            public Sprite(char[] c, Grid.Point xy, Grid.Trajectory t) { constructor(c, xy, t, ConsoleColor.Gray); }
            public Sprite(char[] c, Grid.Point xy, Grid.Trajectory t, ConsoleColor color) { constructor(c, xy, t, color); }

            void constructor(char[] c, Grid.Point xy, Grid.Trajectory t, ConsoleColor color)
            {
                this.Color = color;
                this.Ascii = new List<char>(c).ToArray();
                this.Trail = new Grid.Trail(xy);
                this.Trajectory = t.Clone();
                this.OriginalTrajectory = t.Clone();
            }

            #endregion

        }

        public class Swarm // a collection of sprites
        {

            public List<Sprite> Items = new List<Sprite>();

            bool HasFollowers(Sprite s) { return Leaders().Exists(x => x.Equals(s)); }
            List<Sprite> Followers(Sprite s) { return this.Items.FindAll(x => x.LeaderEquals(s)); }

            List<Sprite> Leaders()
            {
                IEnumerable<Sprite> spriteQuery =
                    from s in this.Items
                    where s.HasLeader
                    select s.Leader;

                return spriteQuery.Distinct().ToList();
            }

            public void Animate()
            {

                // release any followers of dead leaders
                this.Items.FindAll(x => !x.Alive && HasFollowers(x)).ForEach(delegate (Sprite s)
              {
                  Followers(s).ForEach(delegate (Sprite follower) { follower.GoRogue(); });
              });

                // remove dead sprites
                this.Items.FindAll(x => !x.Alive && !x.Active).ForEach(delegate (Sprite s)
               {
                   this.Items.Remove(s);
               });

                // animate squads together
                this.Leaders().ForEach(delegate (Sprite leader)
                {
                    // hide the followers, then animate the leader, then animate the followers without hiding them
                    this.Items.FindAll(x => x.Alive && x.LeaderEquals(leader)).ForEach(delegate (Sprite follower) { follower.Hide(); });
                    leader.Animate();
                    this.Items.FindAll(x => x.Alive && x.LeaderEquals(leader)).ForEach(delegate (Sprite follower) { follower.Animate(false); });
                });

                // animate everyone else
                this.Items.FindAll(x => x.Alive && !x.HasLeader && !HasFollowers(x)).ForEach(delegate (Sprite s) { s.Animate(); });

                // do this for everybody
                this.Items.FindAll(x => x.Active).ForEach(delegate (Sprite s) { s.Activate(); });

                OnAnimated();

                this.Spawn();

            }

            protected virtual void OnAnimated() { }

            public int CollectScore()
            {
                int score = 0;
                this.Items.ForEach(delegate (Sprite s) { score += s.CollectScore(); });
                return score;
            }

            public void TerminateAll() { foreach (Sprite s in this.Items) { s.Terminate(); } }
            public void HideAll() { foreach (Sprite s in this.Items) { s.Hide(); } }

            public void Refresh()
            {
                this.OnRefreshing();
                foreach (Sprite s in this.Items.FindAll(x => x.Alive)) { s.Refresh(); }
                this.OnRefreshed();
            }

            protected virtual void OnRefreshing() { }
            protected virtual void OnRefreshed() { }

            protected virtual void Spawn() { } // do nothing unless inherited class overrides this

            public void CheckCollision(Sprite thatone)
            {
                foreach (Sprite thisone in this.Items.FindAll(x => x.Alive))
                {
                    if (thisone.Hit(thatone)) { thatone.OnHit(thisone.HitEffect); }
                }

            }

            public void CheckCollisions(Swarm otherswarm)
            {
                foreach (Sprite thatone in otherswarm.Items.FindAll(x => x.Alive))
                {
                    this.CheckCollision(thatone);
                }
            }

            #region " Shortcuts "

            public int Count { get { return this.Items.Count; } }

            public bool Empty { get { return this.Items.Count < 1; } }

            public bool Alive { get { return this.Items.Exists(x => x.Alive); } }

            public void Add(Sprite s) { this.Items.Add(s); }

            #endregion

            public Swarm() { }

        }

        public class Complex {

            public List<Swarm> Items = new List<Swarm>();

            public void Animate() {
                foreach (Swarm s in Items) { s.Animate(); }
            }

            public void Add(Swarm s) { this.Items.Add(s); }

            public Complex() {}
        }
    }

    namespace Grid
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

            public Point Clone() { return new Point(this.dX, this.dY); }
            public Point Clone(Trajectory t) { return new Point(this.dX + t.Run, this.dY + t.Rise); }
            public Point Clone(double x, double y) { return new Point(this.dX + x, this.dY + y); }

        }

        public class Trail
        {

            public List<Point> Items = new List<Point>();

            public Point XY { get { return Items[Items.Count - 1]; } } // current XY
            public Point PreviousXY { get { return Items[Items.Count - 2]; } }

            public Point InitialXY { get { return Items[0]; } }

            #region  " Shortcuts "

            public void Add(Point xy) { this.Items.Add(xy); }

            #endregion

            public Trail(Point xy)
            {
                this.Items.Add(xy);
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

            public Trajectory Clone() { return new Trajectory(this.Rise, this.Run, this.Range); }

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

        public static Grid.Point GetCenterCoordinate()
        {
            return new Grid.Point(Easy.Abacus.Round(Screen.Width / 2), Easy.Abacus.Round(Screen.Height / 2));
        }

        public static bool TrySetSize(int targetwidth, int targetheight)
        {
            return TrySetSize(targetwidth, targetheight, true);
        }

        public static bool TrySetSize(int targetwidth, int targetheight, bool adjustmanually)
        {

            try
            {
                // make sure the buffer is never smaller than the window, otherwise the window resize operation will fail
                Console.SetBufferSize(Easy.Abacus.GreaterOf(targetwidth, Console.WindowWidth), Easy.Abacus.GreaterOf(targetheight, Console.WindowHeight));
                // now it's safe to resize everything
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

        public static void TryWrite(Grid.Point xy, char[] s)
        {
            TryWrite(xy.dX, xy.dY, s.ToString());
        }

        public static void TryWrite(Grid.Point xy, string s)
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

        public static void TryWrite(Grid.Point xy, char c)
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

        #endregion
    }

}
