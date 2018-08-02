using AsciiEngine;
using AsciiEngine.Fx;
using AsciiEngine.Grid;
using AsciiEngine.Sprites;
using Easy;

public class Enemy : Sprite
{
    public enum eEnemyType
    {
        Fighter = 0
        , Bomber = 1
        , Interceptor = 2
        , Leader = 3
        , HeavyBomber = 5
        , HeavyFighter = 6
    }

    double ReverseFactor;
    public eEnemyType EnemyType;
    public bool Flown = false;
    public Swarm Messages = new Swarm();
    struct MissileStructure
    {
        public char Ascii;
        public double Range;
        public int MaxCount;
        public MissileStructure(char c, double range, int maxcount)
        {
            this.Ascii = c;
            this.Range = range;
            this.MaxCount = maxcount;
        }
    }
    MissileStructure MissileConfig;

    public Swarm Missiles = new Swarm();

    public Explosion Sparks = new Explosion();
    public Explosion Debris = new Explosion();
    double DebrisRange;

    override public void OnHit()
    {
        this.HP--;

        int scorefactor = (Screen.Height - this.XY.iY) / 4;
        int points = 1;

        Sparks = new Explosion(new string('\x00d7', Abacus.Random.Next(2, 5)).ToCharArray(), this.XY, 0, 2, 1, true, true, true, true);
        if (!this.Alive)
        {
            Debris = new Explosion(this.Ascii, this.XY, this.Width, DebrisRange, 1, true, true, true, true);

            points = 2;
        }
        TieFighterGame.Score += points * scorefactor;

        this.Messages.Items.Add(new Sprite(("+" + points * scorefactor).ToCharArray(), this.XY, new Trajectory(-.5, 0, 4)));
    }

    override public void Activate()
    {

        if (this.Alive)
        {
            // reverse direction
            if (Abacus.Random.NextDouble() < this.ReverseFactor) { this.Trajectory.Run *= -1; }

            // fire
            if (this.Missiles.Items.Count < this.MissileConfig.MaxCount && this.XY.dY > Screen.BottomEdge - MissileConfig.Range && this.HP > 0 && Abacus.RandomTrue)
            {
                this.Missiles.Items.Add(new Sprite(new[] { MissileConfig.Ascii }, this.XY.Clone(this.Width / 2, 0), new Trajectory(1, 0, MissileConfig.Range)));
            }
        }

        this.Missiles.Animate();
        this.Sparks.Animate();
        this.Debris.Animate();
        Messages.Animate();

        if (!this.Alive && this.Debris.Empty && this.Sparks.Empty && this.Messages.Empty && this.Missiles.Empty) { this.Active = false; }

    }

    public Enemy(eEnemyType et) : base()
    {
        this.FlyZone.EdgeMode = FlyZoneClass.eEdgeMode.Bounce;

        double Run = 1;
        double DropsPerRow = 0; // initialized to avoid unassigned variable warning
        switch (et)
        {
            case eEnemyType.Fighter:
                this.Ascii = "|—o—|".ToCharArray();
                this.FlyZone = new FlyZoneClass(0, 2, 0, 0, FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 1;
                this.MissileConfig = new MissileStructure('|', Screen.Height * .33, 1);
                this.DebrisRange = 4;
                DropsPerRow = 8;
                ReverseFactor = .001;
                Run = Abacus.Random.NextDouble() + .5;
                break;
            case eEnemyType.Bomber:
                this.Ascii = "{—o-o—}".ToCharArray();
                this.FlyZone = new FlyZoneClass(Abacus.Round(Screen.Height * .5), Abacus.Round(Screen.Height * .25), Abacus.Round(Screen.Width * -.25), Abacus.Round(Screen.Width * -.25), FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 2;
                this.MissileConfig = new MissileStructure('@', Screen.Height * .85, 2);
                this.DebrisRange = 8;
                DropsPerRow = 1;
                ReverseFactor = 0;
                Run = Abacus.Random.NextDouble() + .5;
                break;
            case eEnemyType.Interceptor:
                this.Ascii = "<—o—>".ToCharArray();
                this.FlyZone = new FlyZoneClass(0, Abacus.Round(Screen.Height * -.15), 0, 0, FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 2;
                this.MissileConfig = new MissileStructure('|', Screen.Height * .40, 1);
                this.DebrisRange = 4;
                DropsPerRow = 16;
                ReverseFactor = .05;
                break;
            case eEnemyType.Leader:
                this.Ascii = "[—o—]".ToCharArray();
                this.FlyZone = new FlyZoneClass(Screen.Height / 4, 5, -2, -2, FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 2;
                this.MissileConfig = new MissileStructure('|', Screen.Height * .33, 2);
                this.DebrisRange = 4;
                DropsPerRow = 20;
                ReverseFactor = .01;
                break;
            case eEnemyType.HeavyBomber:
                this.Ascii = "{=o-o=}".ToCharArray();
                this.FlyZone = new FlyZoneClass(Abacus.Round(Screen.Height * .5), Abacus.Round(Screen.Height * .25), Abacus.Round(Screen.Width * -.25), Abacus.Round(Screen.Width * -.25), FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 3;
                this.MissileConfig = new MissileStructure('@', Screen.Height * .9, 4);
                this.DebrisRange = 10;
                DropsPerRow = 2;
                ReverseFactor = .005;
                break;
            case eEnemyType.HeavyFighter:
                this.Ascii = "|=o=|".ToCharArray();
                this.FlyZone = new FlyZoneClass(0, 2, 0, 0, FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 2;
                this.MissileConfig = new MissileStructure('|', Screen.Height * .25, 1);
                this.DebrisRange = 4;
                DropsPerRow = 15;
                ReverseFactor = .001;
                Run = Abacus.Random.NextDouble() + .5;
                break;
        }

        this.Trail = new Trail(new Point(Abacus.Random.Next(Screen.LeftEdge - this.Width, Screen.RightEdge + this.Width), Screen.TopEdge));

        if (Abacus.RandomTrue) { Run *= -1; }
        this.Trajectory = new Trajectory(DropsPerRow / System.Convert.ToDouble(this.FlyZone.Width), Run);
        this.OriginalTrajectory = this.Trajectory.Clone();
        this.EnemyType = et;

    }

}