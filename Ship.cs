using System;
using System.Collections.Generic;

public class Ship
{
    public enum eShipType { Fighter, Vader, Bomber, Squadron, Interceptor };

    #region " Fly Zone Class "

    class FlyZone
    {

        double _TopOffsetPct;
        double _BottomOffsetPct;
        double _SideOffsetPct;

        public int Top { get { return Convert.ToInt32(Math.Round(AsciiEngine.Height * this._TopOffsetPct, MidpointRounding.AwayFromZero)); } }
        public int Bottom { get { return Convert.ToInt32(Math.Round(AsciiEngine.BottomEdge - AsciiEngine.Height * this._BottomOffsetPct, MidpointRounding.AwayFromZero)); } }
        public int Left { get { return Convert.ToInt32(Math.Round(AsciiEngine.RightEdge * this._SideOffsetPct, MidpointRounding.AwayFromZero)); } }
        public int Right { get { return Convert.ToInt32(Math.Round(AsciiEngine.RightEdge - AsciiEngine.RightEdge * this._SideOffsetPct, MidpointRounding.AwayFromZero)); } }

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

    #region " Properties "

    int _X;
    int _Y = 0;
    int _XDirection = 1;
    int _YDirection = 1;
    string _Ascii;
    int _HP;
    double _SquirrelyFactor;

    FlyZone _flyzone;

    int Width
    {
        get { return this._Ascii.Length; }
    }

    #endregion

    #region " Animation "

    public void Animate()
    {

        bool turnedaround = false;
        AsciiEngine.TryWrite(this._X, this._Y, new String('X', this._Ascii.Length));  // hide it

        if (this._X <= this._flyzone.Left) { this._XDirection = 1; turnedaround = true; }
        if (this._X + this.Width >= this._flyzone.Right) { this._XDirection = -1; turnedaround = true; }
        this._X = this._X + this._XDirection;

        Random r = new Random();

        if (turnedaround || r.Next(100) < (this._SquirrelyFactor * 100)) { this._Y = this._Y + _YDirection; }
        if (this._Y <= this._flyzone.Top) { this._YDirection = 1; }
        if (this._Y >= this._flyzone.Bottom) { this._YDirection = -1; }

        AsciiEngine.TryWrite(this._X, this._Y, this._Ascii);  // show it

    }

    #endregion

    #region " Constructor "

    public Ship(eShipType fightertype)
    {
        switch (fightertype)
        {
            case eShipType.Bomber:
                this._Ascii = "{—o-o—}";
                this._flyzone = new FlyZone(.5, .25, -.25);
                this._SquirrelyFactor = .01;
                break;
            case eShipType.Fighter:
                this._Ascii = "|—o—|";
                this._flyzone = new FlyZone(0, 0, 0);
                this._SquirrelyFactor = .25;
                break;
            case eShipType.Vader:
                this._Ascii = "[—o—]";
                this._flyzone = new FlyZone(.66, 0, .10);
                this._SquirrelyFactor = .1;
                break;
            case eShipType.Squadron:
                this._Ascii = "|—o—|[—o—]|—o—|";
                this._flyzone = new FlyZone(0, .15, .20);
                this._SquirrelyFactor = 0;
                break;
            case eShipType.Interceptor:
                this._Ascii = "<—o—>";
                this._flyzone = new FlyZone(-.15, -.15, 0);
                this._SquirrelyFactor = .4;
                break;
        }

        Random r = new Random();
        this._X = r.Next(0 - this.Width, AsciiEngine.RightEdge + this.Width);
    }

    #endregion

}