using Easy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

namespace AsciiEngine
{
    internal class Leaderboard
    {
        static public string SqlConnectionString;
        internal class Score
        {
            public string Signature;
            public int Points;
            public Score(string sig, int points)
            {
                this.Signature = sig;
                this.Points = points;
            }
        }

        public List<Score> Items = new List<Score>();
        string HighScoreFile { get { return Application.DataPath + "\\highscores.xml"; } }

        public void SqlLoadScores(int count)
        {
            SqlConnection dbConn = new SqlConnection(Leaderboard.SqlConnectionString);
            dbConn.Open();
            SqlCommand oCommand = new SqlCommand("dbo.GetScores", dbConn);
            oCommand.CommandType = System.Data.CommandType.StoredProcedure;
            oCommand.Parameters.AddWithValue("@GameGuid", Application.ID);
            oCommand.Parameters.AddWithValue("@Limit", count);
            SqlDataReader dbReader = oCommand.ExecuteReader();

            if (dbReader.HasRows)
            {
                while (dbReader.Read())
                {
                    this.Items.Add(new Score(dbReader["Signature"].ToString(), Convert.ToInt32(dbReader["Points"])));
                }
            }
            dbConn.Close();
        }

        public void SqlSaveScore(string signature, int points)
        {
            SqlConnection dbConn = new SqlConnection(Leaderboard.SqlConnectionString);
            dbConn.Open();
            SqlCommand oCommand = new SqlCommand("dbo.AddScore", dbConn);
            oCommand.CommandType = System.Data.CommandType.StoredProcedure;
            oCommand.Parameters.AddWithValue("@GameGuid", Application.ID);
            oCommand.Parameters.AddWithValue("@Signature", signature);
            oCommand.Parameters.AddWithValue("@Points", points);
            oCommand.ExecuteNonQuery();
            dbConn.Close();
        }




    }

    internal class Application
    {
        public static string Company { get { return "AsciiMotive"; } }
        public static Guid ID;
        public static string Title = "Untitled";
        public static bool IsWindowsOS { get { return Environment.OSVersion.VersionString.Contains("Windows"); } }
        public static string DataPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" + Company + "\\" + Title; } }

    }
    class Symbol
    {
        public static char ShadeLight { get { return '\x2591'; } }
        public static char TriangleUpSolid { get { return '\x25b2'; } }
        public static char DiamondLight { get { return '\x25ca'; } }
        public static char HyphenDoubleOblique { get { return '\x2e17'; } }
        public static char DotCenter { get { return '\x00b7'; } }
        public static char BarVerticalLeft { get { return '\x258c'; } }
        public static char BarVerticalRight { get { return '\x2590'; } }
        public static char FaceWhite { get { return '\x263a'; } }
        public static char FaceBlack { get { return '\x263b'; } }
    }
    namespace Fx
    {
        public class Explosion : Sprites.Swarm
        {

            public Explosion() { }

            public Explosion(char[] text, Grid.Point coord, int width, double range, double velocity, bool up, bool down, bool left, bool right)
            {
                constructor(text, coord, width, range, velocity, up, down, left, right);
            }

            void constructor(char[] text, Grid.Point coord, int width, double range, double velocity, bool up, bool down, bool left, bool right)
            {

                int position = 0;

                foreach (char c in text)
                {
                    Abacus.Slope slope = Abacus.SlopeFrom(Abacus.RandomDegrees);

                    double throttle = Abacus.Random.NextDouble() + .1; // add a fraction to make sure it's never zero

                    slope.Rise *= velocity * throttle;
                    slope.Run *= velocity * throttle;

                    if (!up && !down) { slope.Rise = 0; } // no rise
                    if (up && !down) { slope.Rise *= -1; } // go up
                    if (!left && !right) { slope.Run = 0; } // no run
                    if (left && !right) { slope.Run *= -1; } // go left

                    Grid.Trajectory t = new Grid.Trajectory(slope.Rise, slope.Run, range);
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

            public void Fill(string[] s) { for (int i = 0; i < s.Length; i++) { this.NewLine(s[i]); } }
            public void Fill(List<string> s) { foreach (string l in s) { this.NewLine(l); } }

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
            public int Width { get { return this.Text.Length; } }
            public bool Blocked = false;

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

            bool HitDetected(Sprite thatone)
            {
                // Hits are only detected if all conditions met:
                // * the other sprite is not this sprite (i.e. we're not crashing into ourselves)
                // * the sprite is still alive
                // * the sprite is active (i.e. still doing something else even if dead)
                // * the given coordinate is within the sprite body
                return (!this.Equals(thatone))
                    && (this.Alive && this.Active)
                    && (thatone.Alive && thatone.Active)
                    && thatone.XY.iY == this.XY.iY
                    && (
                        (thatone.XY.iX >= this.XY.iX && thatone.XY.iX < this.XY.iX + this.Width)
                        || (thatone.XY.iX + thatone.Width >= this.XY.iX && thatone.XY.iX + thatone.Width < this.XY.iX + this.Width)
                        || (thatone.XY.iX <= this.XY.iX && thatone.XY.iX + thatone.Width > this.XY.iX + this.Width)
                    );
            }

            public bool Hit(Sprite thatone)
            {

                bool Collision = this.HitDetected(thatone);

                if (Collision) { OnHit(thatone.HitEffect); }

                return Collision;
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

            public char[] Text;

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
                Screen.TryWrite(this.XY, new String(this.Text));
                this.Shown = true;
                Console.ForegroundColor = savecolor;
            }

            public void Animate(Complex BlockingSwarms) { this.Animate(BlockingSwarms, true); }

            public void Animate(Complex BlockingSwarms, bool hide)
            {
                if (hide) { this.Hide(); }
                var NextCoordinate = this.NextCoordinate();
                this.Trail.Add(NextCoordinate);

                //
                //
                //
                //
                //
                //
                //
                //
                //
                // this is REALLY INEFFICIENT! it loops through every single sprite!
                //
                //
                //
                //
                //
                //
                //
                //
                //
                //
                if (BlockingSwarms != null)
                {
                    foreach (Swarm sw in BlockingSwarms.Items.FindAll(x => x.Alive == true))
                    {
                        foreach (Sprite sp in sw.Items.FindAll(x => x.Alive == true))
                        {
                            if (this.HitDetected(sp)) { this.Trail.Items.Remove(NextCoordinate); }
                        }
                    }
                }
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
                this.Text = new List<char>(c).ToArray();
                this.Trail = new Grid.Trail(xy);
                this.Trajectory = t.Clone();
                this.OriginalTrajectory = t.Clone();
            }

            #endregion

        }

        public class Swarm // a collection of sprites
        {

            public List<Sprite> Items = new List<Sprite>();
            public Complex BlockingSwarms;

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
                    leader.Animate(BlockingSwarms);
                    this.Items.FindAll(x => x.Alive && x.LeaderEquals(leader)).ForEach(delegate (Sprite follower) { follower.Animate(BlockingSwarms, false); });
                });

                // animate everyone else
                this.Items.FindAll(x => x.Alive && !x.Blocked && !x.HasLeader && !HasFollowers(x)).ForEach(delegate (Sprite s) { s.Animate(BlockingSwarms); });

                // do this for everybody
                this.Items.FindAll(x => x.Active).ForEach(delegate (Sprite s)
                {
                    s.Activate();
                    s.Blocked = false;
                });

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

            public void CheckBlocked()
            {
                foreach (Sprite thisone in this.Items.FindAll(x => x.Alive))
                {
                    Grid.Point p = thisone.XY.Clone(thisone.Trajectory);
                    if (this.Items.Exists(thatone => !thatone.Equals(thisone) && thatone.XY.iX == p.iX && thatone.XY.iY == p.iY)) { thisone.Blocked = true; }
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

        public class Complex // a collection of swarms
        {

            public List<Swarm> Items = new List<Swarm>();

            public void Refresh()
            {
                foreach (Swarm s in this.Items.FindAll(x => x.Alive)) { s.Refresh(); }
            }

            public void Animate()
            {

                // remove dead swarms
                this.Items.FindAll(x => !x.Alive).ForEach(delegate (Swarm s)
              {
                  this.Items.Remove(s);
              });

                foreach (Swarm s in Items) { s.Animate(); }
            }

            public void Add(Swarm s) { this.Items.Add(s); }

            public Complex() { }
        }

        public class Static
        {
            public static Complex Swarms = new Complex();
            public static Swarm Sprites = new Swarm();
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

        public static bool TryInitializeScreen(int targetwidth, int targetheight)
        {
            return TryInitializeScreen(targetwidth, targetheight, true);
        }

        public static bool TryInitializeScreen(int targetwidth, int targetheight, bool adjustmanually)
        {

            try
            {
                // make sure the buffer is never smaller than the window, otherwise the window resize operation will fail
                Console.SetBufferSize(Easy.Abacus.GreaterOf(targetwidth, Console.WindowWidth), Easy.Abacus.GreaterOf(targetheight, Console.WindowHeight));
                // now it's safe to resize everything
                Console.SetWindowSize(targetwidth, targetheight);
                Console.SetBufferSize(targetwidth, targetheight);
                System.Console.OutputEncoding = System.Text.Encoding.Unicode;
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

    public class Input
    {
        public static string ArcadeInitials(Grid.Point coord, int maxlength)
        {
            bool iscursorvisible = System.Console.CursorVisible;
            System.Console.OutputEncoding = System.Text.Encoding.Unicode;
            System.Console.CursorVisible = false;

            Screen.TryWrite(coord.iX - 2, coord.iY - 1, "\x2554" + new string('\x2550', maxlength + 2) + "\x2557");
            Screen.TryWrite(coord.iX - 2, coord.iY + 1, "\x255a" + new string('\x2550', maxlength + 2) + "\x255d");
            Screen.TryWrite(coord.iX - 2, coord.iY, "\x2551\x25ba");
            Screen.TryWrite(coord.iX + maxlength, coord.iY, "\x25c4\x2551");

            List<char> alphabet = new List<char>();
            alphabet.Add(' ');
            for (int i = 65; i <= 90; i++) { alphabet.Add(Convert.ToChar(i)); } // A..Z
                                                                                // alphabet.Add('\u0393'); // Γ
                                                                                // alphabet.Add('\u0394'); // Δ
                                                                                // alphabet.Add('\u0398'); // Θ
                                                                                // alphabet.Add('\u039B'); // Λ
                                                                                // alphabet.Add('\u039E'); // Ξ
                                                                                // alphabet.Add('\u03A0'); // Π
                                                                                // alphabet.Add('\u03A3'); // Σ
                                                                                // alphabet.Add('\u03A6'); // Φ
                                                                                // alphabet.Add('\u03A8'); // Ψ
                                                                                // alphabet.Add('\u03A9'); // Ω
            for (int i = 48; i <= 57; i++) { alphabet.Add(Convert.ToChar(i)); } // 0..9
                                                                                // alphabet.Add('\u263a'); // ☺
                                                                                // alphabet.Add('\u263b'); // ☻
                                                                                // alphabet.Add('\u2640'); // ♀
                                                                                // alphabet.Add('\u2642'); // ♂
                                                                                // alphabet.Add('\u2660'); // ♠
                                                                                // alphabet.Add('\u2663'); // ♣
                                                                                // alphabet.Add('\u2665'); // ♥
                                                                                // alphabet.Add('\u2666'); // ♦
                                                                                // alphabet.Add('\u266b'); // ♫

            string initials = "";
            int symbol = 1;
            Easy.Keyboard.EatKeys();
            Screen.TryWrite(coord.iX + initials.Length, coord.iY, alphabet[symbol]);
            do
            {
                Easy.Keyboard.EatKeys(); // don't let keys stack up
                ConsoleKeyInfo k = Console.ReadKey(true);
                if (k.Key == ConsoleKey.UpArrow || k.Key == ConsoleKey.RightArrow)
                {
                    symbol++;
                    if (symbol > alphabet.Count - 1) { symbol = 0; }
                }
                else if (k.Key == ConsoleKey.DownArrow || k.Key == ConsoleKey.LeftArrow)
                {
                    symbol--;
                    if (symbol < 0) { symbol = alphabet.Count - 1; }
                }
                else if (k.Key == ConsoleKey.Spacebar)
                {
                    initials += alphabet[symbol];
                }
                else if (k.Key == ConsoleKey.Escape)
                {
                    initials += new string(' ', maxlength);
                }

                if (initials.Length < maxlength) { Screen.TryWrite(coord.iX + initials.Length, coord.iY, alphabet[symbol]); }

            } while (initials.Length < maxlength);

            System.Console.CursorVisible = iscursorvisible;

            return initials.Substring(0, maxlength);

        }
    }

}
