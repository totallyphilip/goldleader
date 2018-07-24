// this is my first attempt to extend the sprite classes

using AsciiEngine;
using Easy;
using System.Collections.Generic;

public class BadGuy : Sprite
{
    public enum eBadGuyType
    {
        TieFighter
        , TieBomber
    }

    int HP;
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

    public SpriteField Missiles = new SpriteField();

    SpriteField Debris = new SpriteField();
    double DebrisRange;

    public void DoStuff()
    {

        // reverse direction

        if (Numbers.Random.NextDouble() < this.ReverseFactor) { this.Trajectory.Run *= -1; }


        // fire
        if (this.Missiles.Items.Count < this.MissileConfig.MaxCount && this.XY.Y > Screen.BottomEdge - MissileConfig.Range)
        {
            this.Missiles.Items.Add(new Sprite(new[] { MissileConfig.Ascii }, new Screen.Coordinate(this.XY.X + this.Width / 2, this.XY.Y), new Screen.Trajectory(1, 0, MissileConfig.Range)));
        }
        Missiles.Animate();

    }

    public BadGuy(eBadGuyType badguytype) : base()
    {
        this.FlyZone.EdgeMode = FlyZoneClass.eEdgeMode.Bounce;

        double Run = 1;
        double DropsPerRow = 0; // initialized to avoid unassigned variable warning

        switch (badguytype)
        {
            case eBadGuyType.TieFighter:
                this.Ascii = "|—X—|".ToCharArray();
                this.FlyZone = new FlyZoneClass(0, 0, 0, 0, FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 1;
                this.MissileConfig = new MissileStructure('|', 6, 1);
                this.DebrisRange = .5;
                DropsPerRow = 8;
                ReverseFactor = .01;
                break;
            case eBadGuyType.TieBomber:
                this.Ascii = "{—X-X—}".ToCharArray();
                this.FlyZone = new FlyZoneClass(Numbers.Round(Screen.Height * .5), Numbers.Round(Screen.Height * .25), Numbers.Round(Screen.Width * -.25), Numbers.Round(Screen.Width * -.25), FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 2;
                this.MissileConfig = new MissileStructure('@', Screen.Height / 2, 2);
                this.DebrisRange = .5;
                DropsPerRow = 1;
                ReverseFactor = 0;
                break;

        }

        this.Trail = new Screen.CoordinateHistory(new Screen.Coordinate(Numbers.Random.Next(Screen.LeftEdge - this.Width, Screen.RightEdge + this.Width), Screen.TopEdge));

        if (Numbers.Random.NextDouble() < .5) { Run *= -1; }
        this.Trajectory = new Screen.Trajectory(DropsPerRow / System.Convert.ToDouble(this.FlyZone.Width), Run);

    }

}















public class BadGuyField : SpriteField
{
    int MaxBadGuys { get { return 3; } }

    public BadGuyField()
    {
    }

    public void Spawn()
    {
        while (this.Items.Count < this.MaxBadGuys)
        {
            switch (Numbers.Random.Next(0, 2))
            {
                case 0:
                    this.Items.Add(new BadGuy(BadGuy.eBadGuyType.TieFighter));
                    break;
                case 1:
                    this.Items.Add(new BadGuy(BadGuy.eBadGuyType.TieBomber));
                    break;
            }
        }

    }

    protected override void AnimateOverride()
    {
        this.Spawn();
    }


    public void DoStuff()
    {
        foreach (BadGuy badguy in Items.FindAll(x => x.Alive))
        {
            badguy.DoStuff();
        }
    }

}
