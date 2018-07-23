// this is my first attempt to extend the sprite classes

using AsciiEngine;
using System.Collections.Generic;

public class PowerUp : Sprite
{

    public enum ePowerType
    {
        ExtraMissile
        , SaySomething
    }

    string RewardMessage;


    public ePowerType PowerType;

    public void SayStuff()
    {
        System.Console.Write(this.RewardMessage);
    }


    public PowerUp(ePowerType pow, Screen.Coordinate xy, Screen.Trajectory t) : base(xy, t)
    {

        switch (pow)
        {
            case ePowerType.ExtraMissile:
                this.Ascii = new[] { 'M' };
                this.RewardMessage = "Extra Missile";
                break;
            case ePowerType.SaySomething:
                this.Ascii = "just testing".ToCharArray();
                this.RewardMessage = "whatever";
                break;
        }


    }

}