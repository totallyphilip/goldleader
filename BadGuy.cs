// this is my first attempt to extend the sprite classes

using AsciiEngine;
using Easy;
using System.Collections.Generic;

public class BadGuy : Sprite
{

    #region " Fly Zone "

    class FlyZoneClass
    {

        double TopMarginPercent;
        double BottomMarginPercent;
        double SideMarginPercent;

        public int Top { get { return Numbers.Round(Screen.Height * this.TopMarginPercent); } }
        public int Bottom { get { return Numbers.Round(Screen.BottomEdge - Screen.Height * this.BottomMarginPercent); } }
        public int Left { get { return Numbers.Round(Screen.RightEdge * this.SideMarginPercent); } }
        public int Right { get { return Numbers.Round(Screen.RightEdge - Screen.RightEdge * this.SideMarginPercent); } }

        public FlyZoneClass(double toppct, double bottompct, double sidepct)
        {
            this.TopMarginPercent = toppct;
            this.BottomMarginPercent = bottompct;
            this.SideMarginPercent = sidepct;
        }

    }
    FlyZoneClass FlyZone;

    #endregion


    public enum eBadGuyType
    {
        TieFighter
        , TieInterceptor
    }


    public BadGuy(eBadGuyType badguytype) : base()
    {

        /*         switch (badguytype)
                {
                    case eBadGuyType.TieFighter:
                        this.Ascii = "|—o—|".ToCharArray();
                        this.FlyZone = new FlyZoneClass(0, 0, 0);
                        this.SquirrelyFactor = .25;
                        this.HP = 1;
                        this.Missile = new MissileData('|', 6, 1);
                        this.Debris = new DebrisStruct(0.5);
                        break;
                    case ePowerType.SaySomething:
                        this.Ascii = "just testing".ToCharArray();
                        this.RewardMessage = "whatever";
                        break;
                } */


    }

}