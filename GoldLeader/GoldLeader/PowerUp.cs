using Easy;
using AsciiEngine;
using AsciiEngine.Grid;
using AsciiEngine.Sprites;

internal class PowerUp : Sprite
{

    public static int Step = 5;
    public static int FrameDelay = 150;

    public enum ePowerUpType
    {
        Points
        , Shields
        , Missiles
        , Airstrike
        , Jump
        , Torpedo
        , Force
    }
    public ePowerUpType PowerUpType;

    public PowerUp(ePowerUpType type)
    {
        string Symbol = "?"; // init value to eliminate warnings

        this.HitEffect = 0;

        switch (type)
        {
            case ePowerUpType.Points: // extra points
                Symbol = "Score"; //'+';
                break;
            case ePowerUpType.Shields: // deflector shield increase
                Symbol = CharSet.Shield.ToString();
                this.HitEffect = 1;
                break;
            case ePowerUpType.Missiles: // fire an arc of missiles
                Symbol = "Arc"; //CharSet.Missile;
                break;
            case ePowerUpType.Airstrike: // rain down missiles
                Symbol = "Strike"; //CharSet.AirStrike;
                break;
            case ePowerUpType.Jump: // fly up
                Symbol = "Jump"; // CharSet.Jump;
                break;
            case ePowerUpType.Torpedo: // launch explosive
                Symbol = CharSet.Torpedo.ToString();
                break;
            case ePowerUpType.Force: // freeze enemies
                Symbol = "Force";
                break;
        }

        this.Text = (AsciiEngine.Symbol.BarVerticalLeft + Symbol + AsciiEngine.Symbol.BarVerticalRight).ToCharArray();
        this.Color = System.ConsoleColor.Cyan;
        this.FlyZone.EdgeMode = FlyZoneClass.eEdgeMode.Ignore;
        this.Trail = new Trail(new Point(Abacus.Random.Next(Screen.LeftEdge + this.Width, Screen.RightEdge - this.Width), Screen.TopEdge));
        this.Trajectory = new Trajectory(.75, 0, Screen.Height);
        this.OriginalTrajectory = this.Trajectory.Clone();
        this.PowerUpType = type;
        this.HitPoints = 1;
    }

}


