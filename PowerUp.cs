using Easy;
using AsciiEngine;
using AsciiEngine.Grid;
using AsciiEngine.Sprites;

internal class PowerUp : Sprite
{

    public enum ePowerUpType
    {
        Points
        , Shields
        , Missiles
        , Airstrike
        , Warp
    }
    public ePowerUpType PowerUpType;


    public PowerUp(ePowerUpType type)
    {
        string Symbol = "?"; // init value to satisfy the editor

        switch (type)
        {
            case ePowerUpType.Points: // extra points
                Symbol = "+";
                this.HitEffect = 0;
                break;
            case ePowerUpType.Shields: // deflector shield increase
                Symbol = "$";
                this.HitEffect = 1;
                break;
            case ePowerUpType.Missiles: // fire an arc of missiles
                Symbol = "|";
                this.HitEffect = 0;
                break;
            case ePowerUpType.Airstrike: // rain down missiles
                Symbol = "||";
                this.HitEffect = -1;
                break;
            case ePowerUpType.Warp: // fly up
                Symbol = "^";
                this.HitEffect = 0;
                break;

        }

        this.Ascii = ('(' + Symbol + ')').ToCharArray();
        this.FlyZone.EdgeMode = FlyZoneClass.eEdgeMode.Ignore;
        this.Trail = new Trail(new Point(Abacus.Random.Next(Screen.LeftEdge + this.Width, Screen.RightEdge - this.Width), Screen.TopEdge));
        this.Trajectory = new Trajectory(1, 0, Screen.Height);
        this.OriginalTrajectory = this.Trajectory.Clone();
        this.PowerUpType = type;
        this.HitPoints = 1;
    }

}


