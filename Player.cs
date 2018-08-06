using AsciiEngine;
using AsciiEngine.Grid;
using AsciiEngine.Sprites;
using Easy;
using System.Collections.Generic;


public class PlayerMissile : Sprite
{
    override public void OnHit() { this.HP--; }
    public PlayerMissile(AsciiEngine.Grid.Point xy, AsciiEngine.Grid.Trajectory t)
    {
        this.Ascii = new[] { '|' };
        this.Trail = new AsciiEngine.Grid.Trail(xy);
        this.Trajectory = t;
    }
}

public class Player : Sprite
{

    public enum eFlightMode { Maneuver, Attack }
    eFlightMode FlightMode;
    double Run;

    public Swarm Missiles = new Swarm();
    //    public Swarm Messages = new Swarm();

    public int MaxMissiles = 1;
    public AsciiEngine.Fx.Explosion Debris;

    public void ToggleFlightMode()
    {
        if (this.FlightMode == eFlightMode.Attack) { SetFlightMode(eFlightMode.Maneuver); }
        else { SetFlightMode(eFlightMode.Attack); }
        if (this.Trajectory.Run < 0) { GoLeft(); }
        else if (this.Trajectory.Run > 0) { GoRight(); }
    }

    void SetFlightMode(eFlightMode mode)
    {
        if (mode == eFlightMode.Maneuver)
        {
            this.FlightMode = eFlightMode.Maneuver;
            this.Ascii = ".--.".ToCharArray();
            this.Run = 1.5;
        }
        else
        {
            this.FlightMode = eFlightMode.Attack;
            this.Ascii = ":><:".ToCharArray();
            this.Run = .5;
        }
    }

    public void GoLeft() { this.Trajectory.Run = -1 * this.Run; }
    public void GoRight() { this.Trajectory.Run = this.Run; }

    override public void OnHit()
    {
        this.HP--;
        if (this.HP > 0)
        {
            Debris = new AsciiEngine.Fx.Explosion(new string('\x00d7', Abacus.Random.Next(2, 5)).ToCharArray(), this.XY, 0, 2, 1, true, false, true, true);
        }
        else
        {
            Debris = new AsciiEngine.Fx.Explosion(Textify.Repeat("\x00d7*#-", 10).ToCharArray(), this.XY, this.Width, 20, 2, true, false, true, true);
        }

    }

    public Player()
    {
        SetFlightMode(eFlightMode.Maneuver);
        this.FlyZone = new FlyZoneClass(0, 0, 0, 0, FlyZoneClass.eEdgeMode.Stop);
        this.Trajectory = new Trajectory(0, 0);
        this.Trail = new Trail(new Point(Screen.Width / 2 - this.Width / 2, Screen.BottomEdge - 1));
        this.HP = 1;
    }

    public void Fire()
    {
        if (this.FlightMode == eFlightMode.Maneuver)
        {
            if (this.Missiles.Items.Count < this.MaxMissiles)
            {
                PlayerMissile missile = new PlayerMissile(this.XY.Clone(this.Width / 2, 0), new Trajectory(-1, 0, this.XY.dY));
                missile.HP = 1;
                this.Missiles.Items.Add(missile);
            }
        }
        else
        {
            if (this.Missiles.Items.Count == 0)
            {
                double x = (this.XY.dX + this.Width / 2) - this.MaxMissiles / 2;
                for (int i = 0; i < this.MaxMissiles; i++)
                {
                    PlayerMissile missile = new PlayerMissile(new Point(x + i, this.XY.iY), new Trajectory(-1, 0, this.XY.dY));
                    missile.HP = 1;
                    this.Missiles.Items.Add(missile);
                }
            }

        }
    }

    override public void Activate()
    {
        Missiles.Animate();
        //        Messages.Animate();
        if (Debris != null) { Debris.Animate(); }

        if (!this.Alive && this.Debris.Empty && this.Missiles.Empty) { this.Active = false; }
    }

}