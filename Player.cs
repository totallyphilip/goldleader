using AsciiEngine;
using AsciiEngine.Grid;
using AsciiEngine.Sprites;
using Easy;
using System.Collections.Generic;


internal class PlayerMissile : Sprite
{
    override public void OnHit(int damage) { this.HitPoints += damage; }
    public PlayerMissile(AsciiEngine.Grid.Point xy, AsciiEngine.Grid.Trajectory t)
    {
        this.Ascii = new[] { '|' };
        this.Trail = new AsciiEngine.Grid.Trail(xy);
        this.Trajectory = t;
    }
}

internal class Player : Sprite
{

    public enum eFlightMode { Maneuver, Attack }
    eFlightMode FlightMode;
    double Run;
    public int DefaultHitPoints { get { return 5; } }

    public Swarm Missiles = new Swarm();
    //    public Swarm Messages = new Swarm();

    int MyMissileCapacity = 1;
    public int MissileCapacity { get { return this.MyMissileCapacity; } }
    public AsciiEngine.Fx.Explosion Debris;

    public void UpgradeBlasters()
    {
        if (this.MyMissileCapacity < 4) { this.MyMissileCapacity++; }
    }

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
            this.Ascii = "\x00b7==\x00b7".ToCharArray();
            this.Run = 1.5;
        }
        else
        {
            this.FlightMode = eFlightMode.Attack;
            this.Ascii = ":><:".ToCharArray();
            this.Run = .66;
        }
    }

    public void GoLeft() { this.Trajectory.Run = -1 * this.Run; }
    public void GoRight() { this.Trajectory.Run = this.Run; }

    override public void OnHit(int hiteffect)
    {
        this.HitPoints += hiteffect;

        if (hiteffect > 0)
        {
            Debris = new AsciiEngine.Fx.Explosion(new string('$', Abacus.Random.Next(3, 6)).ToCharArray(), this.XY, 0, 3, 1, true, false, true, true);
        }
        else if (hiteffect < 0)
        {
            if (this.HitPoints > 0) { Debris = new AsciiEngine.Fx.Explosion(new string('\x00d7', Abacus.Random.Next(3, 6)).ToCharArray(), this.XY, 0, 3, 1, true, false, true, true); }
            else { Debris = new AsciiEngine.Fx.Explosion(Textify.Repeat("\x00d7*#-", 10).ToCharArray(), this.XY, this.Width, 20, 2, true, false, true, true); }

        }
        else
        {
            Debris = new AsciiEngine.Fx.Explosion(new string('+', Abacus.Random.Next(3, 6)).ToCharArray(), this.XY, 0, 3, 1, true, false, true, true);
        }

    }

    public Player()
    {
        SetFlightMode(eFlightMode.Maneuver);
        this.FlyZone = new FlyZoneClass(0, 1, 0, 0, FlyZoneClass.eEdgeMode.Stop);
        this.Trail = new Trail(new Point(Screen.Width / 2 - this.Width / 2, 0));
        this.HitPoints = this.DefaultHitPoints;
        this.HitEffect = -1;
        this.DropIn();
    }

    public void Fire()
    {
        if (this.FlightMode == eFlightMode.Maneuver)
        {
            if (this.Missiles.Items.Count < this.MyMissileCapacity)
            {
                PlayerMissile missile = new PlayerMissile(this.XY.Clone(this.Width / 2, 0), new Trajectory(-.66, 0, this.XY.dY));
                missile.HitPoints = 1;
                this.Missiles.Items.Add(missile);
            }
        }
        else
        {
            if (this.Missiles.Items.Count == 0)
            {
                double x = (this.XY.dX + this.Width / 2) - this.MyMissileCapacity / 2;
                for (int i = 0; i < this.MyMissileCapacity; i++)
                {
                    PlayerMissile missile = new PlayerMissile(new Point(x + i, this.XY.iY), new Trajectory(-1, 0, this.XY.dY));
                    missile.HitPoints = 1;
                    this.Missiles.Items.Add(missile);
                }
            }

        }
    }

    public void DropIn()
    {
        this.Hide();
        this.Trajectory = new Trajectory(.5, 0);
        this.Trail.Add(new Point(this.XY.dX, Screen.TopEdge));
    }

    public void FireSpread()
    {
        for (double run = -2; run < 2; run += .2)
        {
            PlayerMissile missile = new PlayerMissile(this.XY.Clone(this.Width / 2, 0), new Trajectory(-1, run, Screen.Height / 2));
            missile.HitPoints = 1;
            this.Missiles.Items.Add(missile);
        }
    }

    public void FireAirStrike()
    {
        for (double x = Screen.LeftEdge; x < Screen.RightEdge; x += 4)
        {
            PlayerMissile missile = new PlayerMissile(new Point(x, 0), new Trajectory(1, 0, Screen.Height * .75));
            missile.HitPoints = 1;
            this.Missiles.Items.Add(missile);
        }
    }


    override public void Activate()
    {
        Missiles.Animate();
        if (Debris != null) { Debris.Animate(); }
        if (!this.Alive && this.Debris.Empty && this.Missiles.Empty) { this.Active = false; }
    }

}