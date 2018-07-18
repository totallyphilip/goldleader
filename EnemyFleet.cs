using System;
using System.Collections.Generic;

public class EnemyFleet
{

    #region " Enum "

    public enum eFighterType { Fighter, Vader, Bomber };

    #endregion

    #region " Fighter Class "

    class Fighter
    {

        #region " Fly Zone Class "

        class FlyZone
        {

            int _TopOffset;
            int _BottomOffset;
            int _LeftOffset;
            int _RightOffset;
            bool _Bouncy;

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

        #endregion

        #region " Constructor "

        public Fighter(eFighterType fightertype)
        {
            switch (fightertype)
            {
                case eFighterType.Bomber:
                    this._Ascii = "{—o-o—}".ToCharArray();
                    this._flyzone = new FlyZone(Textify.Height / 2, Textify.Height / 4, true); // lower 1/4 screen
                    break;
                case eFighterType.Fighter:
                    this._Ascii = "|—o—|".ToCharArray();
                    this._flyzone = new FlyZone(); // full screen
                    break;
                case eFighterType.Vader:
                    this._Ascii = "[—o—]".ToCharArray();
                    this._flyzone = new FlyZone(Textify.Height * .66, 0, true); // lower 1/3g screen
                    break;
            }

            Random r = new Random();
            this._X = r.Next(0 - this.Width, Textify.RightEdge + this.Width);
        }

        #endregion

    }

    #endregion

    #region " Properties "

    int _MaxFighters;

    List<Fighter> fighters = new List<Fighter>();

    #endregion

    #region " Methods "

    void RaiseFighterLimit() { this._MaxFighters++; }

    public void Spawn()
    {
        while (fighters.Count < this._MaxFighters)
        {
            fighters.Add(new Fighter(eFighterType.Vader));
        }
    }

    #endregion

    #region " Constructor "
    public EnemyFleet(int maxfighters)
    {
        this._MaxFighters = maxfighters;
    }

    #endregion

}