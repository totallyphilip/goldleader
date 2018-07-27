using AsciiEngine;
using AsciiEngine.Fx;
using AsciiEngine.Coordinates;
using AsciiEngine.Sprites;
using Easy;

public class BadGuy : Sprite
{
    public enum eBadGuyType
    {
        Fighter, Bomber, Interceptor, Master, Squadron
    }

    double ReverseFactor;
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

    Explosion Sparks = new Explosion();
    Explosion Debris = new Explosion();
    double DebrisRange;

    override protected void OnHit()
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

    override public void DoActivities()
    {

        if (this.Alive)
        {
            // reverse direction
            if (Abacus.Random.NextDouble() < this.ReverseFactor) { this.Trajectory.Run *= -1; }

            // fire
            if (this.Missiles.Items.Count < this.MissileConfig.MaxCount && this.XY.dY > Screen.BottomEdge - MissileConfig.Range && this.HP > 0)
            {
                this.Missiles.Items.Add(new Sprite(new[] { MissileConfig.Ascii }, new Point(this.XY.dX + this.Width / 2, this.XY.dY), new Trajectory(1, 0, MissileConfig.Range)));
            }
        }

        this.Missiles.Animate();
        this.Sparks.Animate();
        this.Debris.Animate();
        Messages.Animate();

        if (!this.Alive && this.Debris.Empty && this.Sparks.Empty && this.Messages.Empty && this.Missiles.Empty) { this.Active = false; }

    }

    public BadGuy(eBadGuyType badguytype) : base()
    {
        this.FlyZone.EdgeMode = FlyZoneClass.eEdgeMode.Bounce;

        double Run = 1;
        double DropsPerRow = 0; // initialized to avoid unassigned variable warning
        switch (badguytype)
        {
            case eBadGuyType.Fighter:
                this.Ascii = "|—o—|".ToCharArray();
                this.FlyZone = new FlyZoneClass(0, 2, 0, 0, FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 1;
                this.MissileConfig = new MissileStructure('|', Screen.Height * .33, 1);
                this.DebrisRange = 4;
                DropsPerRow = 8;
                ReverseFactor = .001;
                break;
            case eBadGuyType.Bomber:
                this.Ascii = "{—o-o—}".ToCharArray();
                this.FlyZone = new FlyZoneClass(Abacus.Round(Screen.Height * .5), Abacus.Round(Screen.Height * .25), Abacus.Round(Screen.Width * -.25), Abacus.Round(Screen.Width * -.25), FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 2;
                this.MissileConfig = new MissileStructure('@', Screen.Height * .66, 2);
                this.DebrisRange = 8;
                DropsPerRow = 1;
                ReverseFactor = 0;
                break;
            case eBadGuyType.Interceptor:
                this.Ascii = "<—o—>".ToCharArray();
                this.FlyZone = new FlyZoneClass(0, Abacus.Round(Screen.Height * -.15), 0, 0, FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 2;
                this.MissileConfig = new MissileStructure('|', Screen.Height * .33, 1);
                this.DebrisRange = 4;
                DropsPerRow = 16;
                ReverseFactor = .05;
                break;
            case eBadGuyType.Master:
                this.Ascii = "[—o—]".ToCharArray();
                this.FlyZone = new FlyZoneClass(Screen.Height / 2, 2, 5, 5, FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 3;
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

        this.Trail = new Trail(new Point(Abacus.Random.Next(Screen.LeftEdge - this.Width, Screen.RightEdge + this.Width), Screen.TopEdge));

        if (Abacus.RandomTrue) { Run *= -1; }
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
            switch (Abacus.Random.Next(0, this.MaxBadGuys))
            {
                case 0:
                    this.Items.Add(new BadGuy(BadGuy.eBadGuyType.Fighter));
                    break;
                case 1:
                    this.Items.Add(new BadGuy(BadGuy.eBadGuyType.Bomber));
                    break;
                case 3:
                    this.Items.Add(new BadGuy(BadGuy.eBadGuyType.Interceptor));
                    break;
                case 4:
                    this.Items.Add(new BadGuy(BadGuy.eBadGuyType.Master));
                    break;
                case 5:
                    this.Items.Add(new BadGuy(BadGuy.eBadGuyType.Squadron));
                    break;
                default:
                    this.Items.Add(new BadGuy(BadGuy.eBadGuyType.Fighter));
                    break;
            }
        }

    }

}
