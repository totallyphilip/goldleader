// this is my first attempt to extend the sprite classes

using AsciiEngine;
using Easy;
using System.Collections.Generic;

public class BadGuy : Sprite
{
    public enum eBadGuyType
    {
        TieFighter
        , TieInterceptor
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

        }

        this.Trail = new Screen.CoordinateHistory(new Screen.Coordinate(Numbers.Random.Next(this.FlyZone.LeftEdge - this.Width, this.FlyZone.RightEdge + this.Width), this.FlyZone.TopEdge));

        if (Numbers.Random.NextDouble() < .5) { Run *= -1; }
        this.Trajectory = new Screen.Trajectory(Rise, Run);

    }

}