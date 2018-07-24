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

    struct tMissile
    {
        char Ascii;
        double Range;
        int MaxCount;
        public tMissile(char c, double range, int maxcount)
        {
            this.Ascii = c;
            this.Range = range;
            this.MaxCount = maxcount;
        }
    }
    tMissile MissileTemplate;

    SpriteField Missiles = new SpriteField();

    SpriteField Debris = new SpriteField();
    double DebrisRange;

    public BadGuy(eBadGuyType badguytype) : base()
    {
        this.FlyZone.EdgeMode = FlyZoneClass.eEdgeMode.Bounce;

        double Run = 1;
        double Rise = 0; // initialized to avoid unassigned variable warning

        switch (badguytype)
        {
            case eBadGuyType.TieFighter:
                this.Ascii = "|—X—|".ToCharArray();
                this.FlyZone = new FlyZoneClass(0, 0, 0, 0, FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 1;
                this.MissileTemplate = new tMissile('|', 6, 1);
                this.DebrisRange = .5;
                Rise = 3 / System.Convert.ToDouble(this.FlyZone.Width); // drop 3X per row
                break;
            case eBadGuyType.TieBomber:
                this.Ascii = "{—X-X—}".ToCharArray();
                this.FlyZone = new FlyZoneClass(Numbers.Round(Screen.Height * .5), Numbers.Round(Screen.Height * .25), Numbers.Round(Screen.Width * -.25), Numbers.Round(Screen.Width * -.25), FlyZoneClass.eEdgeMode.Bounce);
                this.HP = 2;
                this.MissileTemplate = new tMissile('@', Screen.Height / 2, 6);
                this.DebrisRange = .5;
                Rise = 1 / System.Convert.ToDouble(this.FlyZone.Width); // drop 3X per row
                break;

        }

        this.Trail = new Screen.CoordinateHistory(new Screen.Coordinate(Numbers.Random.Next(Screen.LeftEdge - this.Width, Screen.RightEdge + this.Width), Screen.TopEdge));

        if (Numbers.Random.NextDouble() < .5) { Run *= -1; }
        this.Trajectory = new Screen.Trajectory(Rise, Run);

    }

}