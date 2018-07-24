using AsciiEngine;
using Easy;
using System;
using System.Collections.Generic;

public class Ship : Sprite
{
    public enum eShipType { Fighter, Vader, Bomber, Squadron, Interceptor };

    #region " Fly Zone "

    new FlyZoneClass FlyZone;

    new class FlyZoneClass
    {

        double _TopOffsetPct;
        double _BottomOffsetPct;
        double _SideOffsetPct;

        public int Top { get { return Convert.ToInt32(System.Math.Round(Screen.Height * this._TopOffsetPct, MidpointRounding.AwayFromZero)); } }
        public int Bottom { get { return Convert.ToInt32(System.Math.Round(Screen.BottomEdge - Screen.Height * this._BottomOffsetPct, MidpointRounding.AwayFromZero)); } }
        public int Left { get { return Convert.ToInt32(System.Math.Round(Screen.RightEdge * this._SideOffsetPct, MidpointRounding.AwayFromZero)); } }
        public int Right { get { return Convert.ToInt32(System.Math.Round(Screen.RightEdge - Screen.RightEdge * this._SideOffsetPct, MidpointRounding.AwayFromZero)); } }

        #region " Constructor "

        public FlyZoneClass(double toff, double boff, double soff)
        {
            this._TopOffsetPct = toff;
            this._BottomOffsetPct = boff;
            this._SideOffsetPct = soff;
        }

        #endregion

    }

    #endregion

    #region " Ship Attributes "

    int HP;
    double SquirrelyFactor;

    #endregion

    #region " Missile Properties "

    struct MissileData
    {
        public char Ascii;
        public int Range;
        public int MaxCount;
        public MissileData(char c, int range, int max)
        {
            this.Ascii = c;
            this.Range = range;
            this.MaxCount = max;
        }
    }

    MissileData Missile;

    public bool Processing { get { return this.MissileField.Items.Count > 0 || this.DebrisField.Items.Count > 0; } }
    public SpriteField MissileField = new SpriteField();
    public SpriteField DebrisField = new SpriteField();

    #endregion

    #region " Status "

    protected override bool AliveOverride { get { return this.HP > 0; } }

    #endregion

    #region " Explody Properties "

    struct DebrisStruct
    {
        public double Range;
        public bool Exploded;
        public DebrisStruct(double range)
        {
            this.Range = range;
            this.Exploded = false;
        }
    }

    DebrisStruct Debris;

    public bool Exploded { get { return this.Debris.Exploded; } }
    public void Explode()
    {
        this.Hide();
        this.Debris.Exploded = true;

        // add ship debris to missiles
        char[] chars = this.Ascii;
        for (int c = 0; c < chars.Length; c++)
        {
            this.DebrisField.Items.Add(new AsciiEngine.Sprite(new[] { chars[c] }, new Screen.Coordinate(this.XY.X + c, this.XY.Y), new Screen.Trajectory(this.Debris.Range)));
        }
        for (int splat = 0; splat < 2; splat++) { this.DebrisField.Items.Add(new AsciiEngine.Sprite(new[] { '*' }, new Screen.Coordinate(this.XY.X + this.Width / 2, this.XY.Y), new Screen.Trajectory(this.Debris.Range * 1.5))); }

    }

    #endregion

    #region " Methods "

    public void AnimateBitsAndPieces()
    {
        this.MissileField.Animate();
        this.DebrisField.Animate();
    }

    public bool Hit(Screen.Coordinate missile)
    {

        if (missile.X >= this.XY.X && missile.X < this.XY.X + this.Width && missile.Y == this.XY.Y)
        {
            // make sparks
            for (int splat = 0; splat < 2; splat++) { this.DebrisField.Items.Add(new AsciiEngine.Sprite(new[] { '\x00d7' }, new Screen.Coordinate(this.XY.X + this.Width / 2, this.XY.Y), new Screen.Trajectory(2))); }
            // reduce health
            this.HP--;
            return true;
        }
        else
        {
            return false;
        }

    }


    protected override Screen.Coordinate NextCoordinate()
    {

        bool turnedaround = false;

        double x;
        double y = this.XY.Y;

        if (this.XY.X <= this.FlyZone.Left) { this.Trajectory.Run = 1; turnedaround = true; }
        if (this.XY.X + this.Width >= this.FlyZone.Right) { this.Trajectory.Run = -1; turnedaround = true; }
        x = this.XY.X + this.Trajectory.Run;

        if (turnedaround || Numbers.Random.Next(100) < (this.SquirrelyFactor * 100)) { y = this.XY.Y + this.Trajectory.Rise; }
        if (this.XY.Y <= this.FlyZone.Top) { this.Trajectory.Rise = 1; }
        if (this.XY.Y >= this.FlyZone.Bottom) { this.Trajectory.Rise = -1; }

        // fire!
        // must be near the bottom, have more missiles, and not fire every time
        if (this.MissileField.Items.Count < this.Missile.MaxCount && this.XY.Y + this.Missile.Range >= Screen.BottomEdge && Numbers.Random.NextDouble() < .2)
        {
            this.MissileField.Items.Add(new Sprite(new[] { this.Missile.Ascii }, new Screen.Coordinate(this.XY.X + this.Width / 2, this.XY.Y), new Screen.Trajectory(0, 1, this.Missile.Range)));
        }

        return new Screen.Coordinate(x, y);

    }

    #endregion

    #region " Constructor "

    public Ship(eShipType fightertype) : base()
    {
        switch (fightertype)
        {
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
        }

        this.Trail = new Screen.CoordinateHistory(new Screen.Coordinate(Numbers.Random.Next(0 - this.Width, Screen.RightEdge + this.Width), 0));



        int Run;
        if (Numbers.Random.NextDouble() < .5) { Run = 1; } else { Run = -1; }
        this.Trajectory = new Screen.Trajectory(Run, 1);

    }

    #endregion

}