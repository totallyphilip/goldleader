using AsciiEngine;
using Easy;
using System;
using System.Collections.Generic;

public class Ship
{
    public enum eShipType { Fighter, Vader, Bomber, Squadron, Interceptor };

    #region " Fly Zone "

    FlyZone _FlyZone;

    class FlyZone
    {

        double _TopOffsetPct;
        double _BottomOffsetPct;
        double _SideOffsetPct;

        public int Top { get { return Convert.ToInt32(System.Math.Round(Screen.Height * this._TopOffsetPct, MidpointRounding.AwayFromZero)); } }
        public int Bottom { get { return Convert.ToInt32(System.Math.Round(Screen.BottomEdge - Screen.Height * this._BottomOffsetPct, MidpointRounding.AwayFromZero)); } }
        public int Left { get { return Convert.ToInt32(System.Math.Round(Screen.RightEdge * this._SideOffsetPct, MidpointRounding.AwayFromZero)); } }
        public int Right { get { return Convert.ToInt32(System.Math.Round(Screen.RightEdge - Screen.RightEdge * this._SideOffsetPct, MidpointRounding.AwayFromZero)); } }

        #region " Constructor "

        public FlyZone(double toff, double boff, double soff)
        {
            this._TopOffsetPct = toff;
            this._BottomOffsetPct = boff;
            this._SideOffsetPct = soff;
        }

        #endregion

    }

    #endregion

    #region " Movement Properties "

    int _X;
    int _Y = 0;
    int _XDirection = 1;
    int _YDirection = 1;
    int _HP;
    double _SquirrelyFactor;

    #endregion

    #region " Missile Properties "

    char _MissileAscii;
    int _MissileRange;
    int _MissileLimit;
    SpriteField _MissileField = new SpriteField();
    public bool Firing { get { return this._MissileField.Sprites.Count > 0; } }
    public SpriteField MissileField { get { return this._MissileField; } }

    #endregion

    #region " General Properties "

    string _Ascii;
    int Width { get { return this._Ascii.Length; } }
    public bool Alive { get { return this._HP > 0; } }

    #endregion

    #region " Explody Properties "

    double _DebrisRange;
    bool _Exploded = false;
    public bool Exploded { get { return this._Exploded; } }
    public void Explode()
    {
        this.Hide();
        this._Exploded = true;

        // add ship debris to missiles
        char[] chars = this._Ascii.ToCharArray();
        for (int c = 0; c < chars.Length; c++)
        {
            this.MissileField.Sprites.Add(new AsciiEngine.Sprite(chars[c], this._X + c, this._Y, this._DebrisRange));
        }
        for (int splat = 0; splat < 2; splat++) { this.MissileField.Sprites.Add(new AsciiEngine.Sprite('*', this._X + this.Width / 2, this._Y, this._DebrisRange * 1.5)); }

    }

    #endregion

    #region " Methods "

    public bool Hit(double x, double y)
    {
        x = Numbers.Round(x);
        y = Numbers.Round(y);
        if (x >= this._X && x < this._X + this.Width && y == this._Y)
        {
            // make sparks
            for (int splat = 0; splat < 2; splat++) { this.MissileField.Sprites.Add(new AsciiEngine.Sprite('\x00d7', this._X + this.Width / 2, this._Y, 2)); }
            // reduce health
            this._HP--;
            return true;
        }
        else
        {
            return false;
        }

    }

    void Hide()
    {
        Screen.TryWrite(this._X, this._Y, new String(' ', this._Ascii.Length));
    }

    public void Animate()
    {

        bool turnedaround = false;
        this.Hide();

        if (this._X <= this._FlyZone.Left) { this._XDirection = 1; turnedaround = true; }
        if (this._X + this.Width >= this._FlyZone.Right) { this._XDirection = -1; turnedaround = true; }
        this._X = this._X + this._XDirection;

        if (turnedaround || Numbers.Random.Next(100) < (this._SquirrelyFactor * 100)) { this._Y = this._Y + _YDirection; }
        if (this._Y <= this._FlyZone.Top) { this._YDirection = 1; }
        if (this._Y >= this._FlyZone.Bottom) { this._YDirection = -1; }

        Screen.TryWrite(this._X, this._Y, this._Ascii);  // show it

        // fire!
        // must be near the bottom, have more missiles, and not fire every time
        if (this.MissileField.Sprites.Count < this._MissileLimit && this._Y + this._MissileRange >= Screen.BottomEdge && Numbers.Random.NextDouble() < .2)
        {
            this.MissileField.Sprites.Add(new Sprite(this._MissileAscii, this._X + this.Width / 2, this._Y, 0, 1, _MissileRange));
        }

    }

    #endregion

    #region " Constructor "

    public Ship(eShipType fightertype)
    {
        switch (fightertype)
        {
            case eShipType.Fighter:
                this._Ascii = "|—o—|";
                this._FlyZone = new FlyZone(0, 0, 0);
                this._SquirrelyFactor = .25;
                this._HP = 1;
                this._MissileAscii = '|';
                this._MissileRange = 6;
                this._MissileLimit = 1;
                this._DebrisRange = 0.5;
                break;
            case eShipType.Bomber:
                this._Ascii = "{—o-o—}";
                this._FlyZone = new FlyZone(.5, .25, -.25);
                this._SquirrelyFactor = .01;
                this._HP = 2;
                this._MissileAscii = '@';
                this._MissileRange = Screen.Height / 2;
                this._MissileLimit = 1;
                this._DebrisRange = 6;
                break;
            case eShipType.Interceptor:
                this._Ascii = "<—o—>";
                this._FlyZone = new FlyZone(-.15, -.15, 0);
                this._SquirrelyFactor = .4;
                this._HP = 2;
                this._MissileAscii = '|';
                this._MissileRange = 6;
                this._MissileLimit = 2;
                this._DebrisRange = 1;
                break;
            case eShipType.Vader:
                this._Ascii = "[—o—]";
                this._FlyZone = new FlyZone(.66, 0, .10);
                this._SquirrelyFactor = .1;
                this._HP = 3;
                this._MissileAscii = '|';
                this._MissileRange = 10;
                this._MissileLimit = 3;
                this._DebrisRange = 1;
                break;
            case eShipType.Squadron:
                this._Ascii = "|—o—|[—o—]|—o—|";
                this._FlyZone = new FlyZone(0, .15, .20);
                this._SquirrelyFactor = 0;
                this._HP = 6;
                this._MissileAscii = '|';
                this._MissileRange = 6;
                this._MissileLimit = 5;
                this._DebrisRange = 8;
                break;
        }

        this._X = Numbers.Random.Next(0 - this.Width, Screen.RightEdge + this.Width);
    }

    #endregion

}