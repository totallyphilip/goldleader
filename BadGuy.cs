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

    public void MakeDebris()
    {
        for (int i = 0; i < Numbers.Random.Next(1, 4); i++)
        {
            Debris.Items.Add(new Sprite(new[] { '\x00d7' }, new Screen.Coordinate(this.XY.X + this.Width / 2, this.XY.Y), new Screen.Trajectory(2)));
        }

        if (!this.Alive)
        {
            for (int c = 0; c < this.Width; c++)
            {
                this.Debris.Items.Add(new Sprite(new[] { this.Ascii[c] }, new Screen.Coordinate(this.XY.X + c, this.XY.Y), new Screen.Trajectory(this.DebrisRange)));
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
                this.Missiles.Items.Add(new Sprite(new[] { MissileConfig.Ascii }, new Screen.Coordinate(this.XY.X + this.Width / 2, this.XY.Y), new Screen.Trajectory(1, 0, MissileConfig.Range)));
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
                this.FlyZone = new FlyZoneClass(0, 0, 0, 0, FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 1;
                this.MissileConfig = new MissileStructure('|', 6, 1);
                this.DebrisRange = 4;
                DropsPerRow = 8;
                ReverseFactor = .01;
                break;
            case eBadGuyType.TieBomber:
                this.Ascii = "{—o-o—}".ToCharArray();
                this.FlyZone = new FlyZoneClass(Numbers.Round(Screen.Height * .5), Numbers.Round(Screen.Height * .25), Numbers.Round(Screen.Width * -.25), Numbers.Round(Screen.Width * -.25), FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 2;
                this.MissileConfig = new MissileStructure('@', Screen.Height / 2, 2);
                this.DebrisRange = 8;
                DropsPerRow = 1;
                ReverseFactor = 0;
                break;

                /* 
                            case eShipType.Fighter:
                                this.Ascii = "|—o—|".ToCharArray();
                                this.FlyZone = new FlyZoneClass(0, 0, 0);
                                this.SquirrelyFactor = .25;
                                this.HP = 1;
                                this.Missile = new MissileData('|', 6, 1);
                                this.Debris = new DebrisStruct(0.5);
                                break;
                            case eShipType.Bomber:
                                this.Ascii = "{—o-o—}".ToCharArray();
                                this.FlyZone = new FlyZoneClass(.5, .25, -.25);
                                this.SquirrelyFactor = .01;
                                this.HP = 2;
                                this.Missile = new MissileData('@', Screen.Height / 2, 1);
                                this.Debris = new DebrisStruct(6);
                                break;
                            case eShipType.Interceptor:
                                this.Ascii = "<—o—>".ToCharArray();
                                this.FlyZone = new FlyZoneClass(-.15, -.15, 0);
                                this.SquirrelyFactor = .4;
                                this.HP = 2;
                                this.Missile = new MissileData('|', 6, 2);
                                this.Debris = new DebrisStruct(1);
                                break;
                            case eShipType.Vader:
                                this.Ascii = "[—o—]".ToCharArray();
                                this.FlyZone = new FlyZoneClass(.66, 0, .10);
                                this.SquirrelyFactor = .1;
                                this.HP = 3;
                                this.Missile = new MissileData('|', 10, 3);
                                this.Debris = new DebrisStruct(1);
                                break;
                            case eShipType.Squadron:
                                this.Ascii = "|—o—|[—o—]|—o—|".ToCharArray();
                                this.FlyZone = new FlyZoneClass(0, .15, .20);
                                this.SquirrelyFactor = 0;
                                this.HP = 6;
                                this.Missile = new MissileData('|', 6, 5);
                                this.Debris = new DebrisStruct(8);
                                break;
                 */
        }

        this.Trail = new Screen.CoordinateHistory(new Screen.Coordinate(Numbers.Random.Next(Screen.LeftEdge - this.Width, Screen.RightEdge + this.Width), Screen.TopEdge));

        if (Numbers.Random.NextDouble() < .5) { Run *= -1; }
        this.Trajectory = new Screen.Trajectory(DropsPerRow / System.Convert.ToDouble(this.FlyZone.Width), Run);

    }

}















public class BadGuyField : SpriteField
{
    int MaxBadGuys { get { return 3; } }

    public BadGuyField() { }

    protected override void Spawn()
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

}
