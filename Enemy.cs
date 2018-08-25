using AsciiEngine;
using AsciiEngine.Fx;
using AsciiEngine.Grid;
using AsciiEngine.Sprites;
using Easy;

internal class Enemy : Sprite
{
    public enum eEnemyType
    {
        Fighter
        , Bomber
        , Interceptor
        , Leader
        , HeavyBomber
        , HeavyFighter
        , Vanguard
        , Interdictor
    }

    double ReverseFactor;
    public eEnemyType EnemyType;
    int InitialHitPoints;
    bool DemoMode;

    //    public Swarm Messages = new Swarm();
    struct MissileStructure
    {
        public char Text;
        public double Range;
        public int MaxCount;
        public MissileStructure(char c, double range, int maxcount)
        {
            this.Text = c;
            this.Range = range;
            this.MaxCount = maxcount;
        }
    }
    MissileStructure MissileConfig;

    public Swarm Missiles = new Swarm();

    double DebrisRange;

    override public void OnHit(int damage)
    {
        this.HitPoints += damage;

        int scorefactor = (Screen.Height - this.XY.iY) / 4;
        int points = 1;

        AsciiEngine.Sprites.Static.Swarms.Add(new Explosion(new string(CharSet.Debris, this.Width).ToCharArray(), this.XY, this.Width, 3, 1, true, true, true, true));
        if (this.Alive)
        {
            this.Text[Abacus.Random.Next(this.Text.Length)] = CharSet.Damage;
            this.Trail.Add(this.XY.Clone(0, -1));
            this.Trajectory.Run *= -.75;
        }
        else
        {

            AsciiEngine.Sprites.Static.Swarms.Add(new Explosion(this.Text, this.XY, this.Width, DebrisRange, 1, true, true, true, true));

            points = 2;
        }

        this.Score = (points * scorefactor);

        if (DemoMode) { UnicodeWars.ScoreUp(this.Score, this.XY); }

    }

    override public void Activate()
    {

        if (this.Alive)
        {

            // add smoke
            if (this.HitPoints < this.InitialHitPoints && Abacus.Random.NextDouble() > .8)
            {
                AsciiEngine.Sprites.Static.Swarms.Add(new Explosion(new string(CharSet.Smoke, this.InitialHitPoints - this.HitPoints).ToCharArray(), this.XY.Clone(this.Width / 2, 0), 0, 2, .5, true, true, true, true));
            }

            // reverse direction
            if (Abacus.Random.NextDouble() < this.ReverseFactor) { this.Trajectory.Run *= -1; }

            // fire
            if (this.Missiles.Items.Count < this.MissileConfig.MaxCount && this.XY.dY > Screen.BottomEdge - MissileConfig.Range && this.HitPoints > 0 && Abacus.RandomTrue)
            {
                this.Missiles.Items.Add(new Sprite(new[] { MissileConfig.Text }, this.XY.Clone(this.Width / 2, 0), new Trajectory(1, 0, MissileConfig.Range), System.ConsoleColor.Green));
            }
        }

        this.Missiles.Animate();
        //Messages.Animate();

        if (!this.Alive && this.Missiles.Empty) { this.Active = false; }

    }

    public Enemy(eEnemyType et, bool demo) : base()
    {
        this.FlyZone.EdgeMode = FlyZoneClass.eEdgeMode.Bounce;

        this.DemoMode = demo;

        double Run = 1;
        double DropsPerRow = 0; // initialized to avoid unassigned variable warning
        switch (et)
        {
            case eEnemyType.Fighter:
                this.Text = "|—o—|".ToCharArray();
                this.FlyZone = new FlyZoneClass(0, 7, 0, 0, FlyZoneClass.eEdgeMode.Bounce);
                this.HitPoints = 1;
                this.MissileConfig = new MissileStructure('|', Screen.Height * .33, 1);
                this.DebrisRange = 4;
                DropsPerRow = 8;
                ReverseFactor = .001;
                Run = Abacus.Random.NextDouble() + .5;
                break;
            case eEnemyType.Bomber:
                this.Text = "{—o-o—}".ToCharArray();
                this.FlyZone = new FlyZoneClass(Abacus.Round(Screen.Height * .5), Abacus.Round(Screen.Height * .25), Abacus.Round(Screen.Width * -.25), Abacus.Round(Screen.Width * -.25), FlyZoneClass.eEdgeMode.Bounce);
                this.HitPoints = 2;
                this.MissileConfig = new MissileStructure('@', Screen.Height * .85, 2);
                this.DebrisRange = 8;
                DropsPerRow = 1;
                ReverseFactor = 0;
                Run = Abacus.Random.NextDouble() + .5;
                break;
            case eEnemyType.Interceptor:
                this.Text = "<—o—>".ToCharArray();
                this.FlyZone = new FlyZoneClass(0, Abacus.Round(Screen.Height * -.15), 0, 0, FlyZoneClass.eEdgeMode.Bounce);
                this.HitPoints = 2;
                this.MissileConfig = new MissileStructure('|', Screen.Height * .40, 1);
                this.DebrisRange = 4;
                DropsPerRow = 16;
                ReverseFactor = .05;
                break;
            case eEnemyType.Leader:
                this.Text = "[—o—]".ToCharArray();
                this.FlyZone = new FlyZoneClass(Screen.Height / 4, 5, -2, -2, FlyZoneClass.eEdgeMode.Bounce);
                this.HitPoints = 2;
                this.MissileConfig = new MissileStructure('|', Screen.Height * .33, 2);
                this.DebrisRange = 4;
                DropsPerRow = 20;
                ReverseFactor = .01;
                break;
            case eEnemyType.HeavyBomber:
                this.Text = "{-o-8-}".ToCharArray();
                this.FlyZone = new FlyZoneClass(Abacus.Round(Screen.Height * .5), Abacus.Round(Screen.Height * .25), Abacus.Round(Screen.Width * -.25), Abacus.Round(Screen.Width * -.25), FlyZoneClass.eEdgeMode.Bounce);
                this.HitPoints = 3;
                this.MissileConfig = new MissileStructure('@', Screen.Height * .9, 4);
                this.DebrisRange = 10;
                DropsPerRow = 2;
                ReverseFactor = .005;
                break;
            case eEnemyType.HeavyFighter:
                this.Text = "|=o=|".ToCharArray();
                this.FlyZone = new FlyZoneClass(0, 2, 0, 0, FlyZoneClass.eEdgeMode.Bounce);
                this.HitPoints = 2;
                this.MissileConfig = new MissileStructure('|', Screen.Height * .25, 1);
                this.DebrisRange = 4;
                DropsPerRow = 15;
                ReverseFactor = .001;
                Run = Abacus.Random.NextDouble() + .5;
                break;
            case eEnemyType.Vanguard:
                this.Text = "\\-o-/".ToCharArray();
                this.FlyZone = new FlyZoneClass(0, 5, -5, -5, FlyZoneClass.eEdgeMode.Bounce);
                this.HitPoints = 3;
                this.MissileConfig = new MissileStructure('|', 6, 1);
                this.DebrisRange = 4;
                DropsPerRow = 15;
                ReverseFactor = .001;
                Run = Abacus.Random.NextDouble() + .5;
                break;
            case eEnemyType.Interdictor:
                this.Text = "{-8o8-}".ToCharArray();
                this.FlyZone = new FlyZoneClass(0, Abacus.Round(Screen.Height * .75), Abacus.Round(Screen.Width * -.25), Abacus.Round(Screen.Width * -.25), FlyZoneClass.eEdgeMode.Bounce);
                this.HitPoints = 4;
                this.MissileConfig = new MissileStructure('@', Screen.Height * .9, 6);
                this.DebrisRange = 10;
                DropsPerRow = 2;
                ReverseFactor = .005;
                break;
        }

        this.Color = System.ConsoleColor.White;
        this.Trail = new Trail(new Point(Abacus.Random.Next(Screen.LeftEdge - this.Width, Screen.RightEdge + this.Width), Screen.TopEdge));

        if (Abacus.RandomTrue) { Run *= -1; }
        this.Trajectory = new Trajectory(DropsPerRow / System.Convert.ToDouble(this.FlyZone.Width), Run);
        this.OriginalTrajectory = this.Trajectory.Clone();
        this.EnemyType = et;
        this.InitialHitPoints = this.HitPoints;

    }

}