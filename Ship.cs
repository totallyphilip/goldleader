using System;
using System.Collections.Generic;

public class Ship
{
    public enum eShipType { Fighter, Vader, Bomber };

    #region " Fly Zone Class "

    class FlyZone
    {

        int _TopOffset;
        int _BottomOffset;
        int _LeftOffset;
        int _RightOffset;
        bool _Bouncy;

        public int Top { get { return this._TopOffset; } }
        public int Bottom { get { return Textify.BottomEdge - this._BottomOffset; } }
        public int Left { get { return this._LeftOffset; } }
        public int Right { get { return Textify.RightEdge - this._RightOffset; } }

        #region " Constructor "

        public FlyZone() : this(0, 0, 0, 0, false) { }

        public FlyZone(double toff, double boff, bool bouncy) : this(toff, boff, 0, 0, bouncy) { }

        public FlyZone(double toff, double boff, double loff, double roff, bool bouncy)
        {
            this._TopOffset = Convert.ToInt32(Math.Round(toff, MidpointRounding.AwayFromZero));
            this._BottomOffset = Convert.ToInt32(Math.Round(boff, MidpointRounding.AwayFromZero));
            this._LeftOffset = Convert.ToInt32(Math.Round(loff, MidpointRounding.AwayFromZero)); ;
            this._RightOffset = Convert.ToInt32(Math.Round(roff, MidpointRounding.AwayFromZero)); ;
            this._Bouncy = bouncy;
        }

        #endregion

    }

    #endregion

    #region " Properties "

    int _X;
    int _Y = 0;
    int _XDirection = 1;
    int _YDirection = 1;
    char[] _Ascii;
    int _HP;

    FlyZone _flyzone;

    int Width
    {
        get { return this._Ascii.Length; }
    }

    #endregion

    #region " Methods "

    void MoveTo(int x, int y)
    {
        this._X = x;
        this._Y = y;
    }

    public void Hide()
    {
        for (int c = 0; c < this.Width; c++) { Textify.SayInbounds(this._X + c, this._Y, ' '); }
    }
    public void Show()
    {
        for (int c = 0; c < this.Width; c++) { Textify.SayInbounds(this._X + c, this._Y, this._Ascii[c]); }
    }

    public void Animate()
    {

        bool _XReverse = false;
        this.Hide();
        if (this._X < this._flyzone.Left - 2) { this._XDirection = 1; _XReverse = true; }
        if (this._X + this.Width > this._flyzone.Right + 2) { this._XDirection = -1; _XReverse = true; }
        if (_XReverse) { this._Y = this._Y + _YDirection; }
        if (this._Y <= this._flyzone.Top) { this._YDirection = 1; }
        if (this._Y >= this._flyzone.Bottom) { this._YDirection = -1; }
        this._X = this._X + this._XDirection;

        this.Show();




    }

    #endregion

    #region " Constructor "

    public Ship(eShipType fightertype)
    {
        switch (fightertype)
        {
            case eShipType.Bomber:
                this._Ascii = "{—o-o—}".ToCharArray();
                this._flyzone = new FlyZone(Textify.Height / 2, Textify.Height / 4, true); // lower 1/4 screen
                break;
            case eShipType.Fighter:
                this._Ascii = "|—o—|".ToCharArray();
                this._flyzone = new FlyZone(); // full screen
                break;
            case eShipType.Vader:
                this._Ascii = "[—o—]".ToCharArray();
                this._flyzone = new FlyZone(Textify.Height * .66, 0, true); // lower 1/3g screen
                break;
        }

        Random r = new Random();
        this._X = r.Next(0 - this.Width, Textify.RightEdge + this.Width);
    }

    #endregion

}