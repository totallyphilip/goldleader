using AsciiEngine;
using AsciiEngine.Coordinates;
using AsciiEngine.Sprites;
using Easy;

public class BadGuy : Sprite
{
    public enum eBadGuyType
    {
        TieFighter, TieBomber, TieInterceptor, Vader, Squadron
    }

    double ReverseFactor;

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

    Swarm Debris = new Swarm();
    double DebrisRange;

    public void MakeDebris()
    {
        for (int i = 0; i < Numbers.Random.Next(1, 4); i++)
        {
            Debris.Items.Add(new Sprite(new[] { '\x00d7' }, new Coordinate(this.XY.X + this.Width / 2, this.XY.Y), new Trajectory(2)));
        }

        if (!this.Alive)
        {
            for (int c = 0; c < this.Width; c++)
            {
                this.Debris.Items.Add(new Sprite(new[] { this.Ascii[c] }, new Coordinate(this.XY.X + c, this.XY.Y), new Trajectory(this.DebrisRange)));
            }
        }
    }

    override public void DoActivities()
    {

        if (this.Alive)
        {
            // reverse direction
            if (Numbers.Random.NextDouble() < this.ReverseFactor) { this.Trajectory.Run *= -1; }

            // fire
            if (this.Missiles.Items.Count < this.MissileConfig.MaxCount && this.XY.Y > Screen.BottomEdge - MissileConfig.Range && this.HP > 0)
            {
                this.Missiles.Items.Add(new Sprite(new[] { MissileConfig.Ascii }, new Coordinate(this.XY.X + this.Width / 2, this.XY.Y), new Trajectory(1, 0, MissileConfig.Range)));
            }
        }

        this.Missiles.Animate();
        this.Debris.Animate();

        if (!this.Alive && this.Debris.Items.Count < 1 && this.Missiles.Items.Count < 1) { this.Active = false; }

    }

    public BadGuy(eBadGuyType badguytype) : base()
    {
        this.FlyZone.EdgeMode = FlyZoneClass.eEdgeMode.Bounce;

        double Run = 1;
        double DropsPerRow = 0; // initialized to avoid unassigned variable warning

        switch (badguytype)
        {
            case eBadGuyType.TieFighter:
                this.Ascii = "|—o—|".ToCharArray();
                this.FlyZone = new FlyZoneClass(0, 2, 0, 0, FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 1;
                this.MissileConfig = new MissileStructure('|', Screen.Height * .33, 1);
                this.DebrisRange = 4;
                DropsPerRow = 8;
                ReverseFactor = .001;
                break;
            case eBadGuyType.TieBomber:
                this.Ascii = "{—o-o—}".ToCharArray();
                this.FlyZone = new FlyZoneClass(Numbers.Round(Screen.Height * .5), Numbers.Round(Screen.Height * .25), Numbers.Round(Screen.Width * -.25), Numbers.Round(Screen.Width * -.25), FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 2;
                this.MissileConfig = new MissileStructure('@', Screen.Height * .66, 2);
                this.DebrisRange = 8;
                DropsPerRow = 1;
                ReverseFactor = 0;
                break;
            case eBadGuyType.TieInterceptor:
                this.Ascii = "<—o—>".ToCharArray();
                this.FlyZone = new FlyZoneClass(0, Numbers.Round(Screen.Height * -.15), 0, 0, FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 2;
                this.MissileConfig = new MissileStructure('|', Screen.Height * .33, 1);
                this.DebrisRange = 4;
                DropsPerRow = 16;
                ReverseFactor = .05;
                break;
            case eBadGuyType.Vader:
                this.Ascii = "[—o—]".ToCharArray();
                this.FlyZone = new FlyZoneClass(Screen.Height / 2, 2, 5, 5, FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 2;
                this.MissileConfig = new MissileStructure('|', Screen.Height * .25, 2);
                this.DebrisRange = 4;
                DropsPerRow = 16;
                ReverseFactor = .01;
                break;
            case eBadGuyType.Squadron:
                this.Ascii = "|—o—|[—o—]|—o—|".ToCharArray();
                this.FlyZone = new FlyZoneClass(0, 5, Screen.Width / 4, Screen.Width / 4, FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 6;
                this.MissileConfig = new MissileStructure('|', Screen.Height * .5, 3);
                this.DebrisRange = 8;
                DropsPerRow = 2;
                ReverseFactor = 0;
                break;
        }

        this.Trail = new Trail(new Coordinate(Numbers.Random.Next(Screen.LeftEdge - this.Width, Screen.RightEdge + this.Width), Screen.TopEdge));

        if (Numbers.Random.NextDouble() < .5) { Run *= -1; }
        this.Trajectory = new Trajectory(DropsPerRow / System.Convert.ToDouble(this.FlyZone.Width), Run);

    }

}















public class BadGuyField : Swarm
{

    System.DateTime starttime = System.DateTime.Now;
    public int MaxBadGuys
    {
        get
        {
            System.DateTime rightnow = System.DateTime.Now;
            System.TimeSpan span = rightnow.Subtract(starttime);
            return 1 + System.Convert.ToInt32(span.TotalSeconds) / 30; // another bad guy every N seconds
        }
    }

    public BadGuyField()
    {

        // foreach (BadGuy.eBadGuyType ship in (BadGuy.eBadGuyType[])System.Enum.GetValues(typeof(BadGuy.eBadGuyType))) { }


    }

    protected override void Spawn()
    {
        while (this.Items.Count < this.MaxBadGuys)
        {
            switch (Numbers.Random.Next(0, this.MaxBadGuys))
            {
                case 0:
                    this.Items.Add(new BadGuy(BadGuy.eBadGuyType.TieFighter));
                    break;
                case 1:
                    this.Items.Add(new BadGuy(BadGuy.eBadGuyType.TieBomber));
                    break;
                case 3:
                    this.Items.Add(new BadGuy(BadGuy.eBadGuyType.TieInterceptor));
                    break;
                case 4:
                    this.Items.Add(new BadGuy(BadGuy.eBadGuyType.Vader));
                    break;
                case 5:
                    this.Items.Add(new BadGuy(BadGuy.eBadGuyType.Squadron));
                    break;
                default:
                    this.Items.Add(new BadGuy(BadGuy.eBadGuyType.TieFighter));
                    break;
            }
        }

    }

}
