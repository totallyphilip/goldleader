using UnicodeEngine;
using UnicodeEngine.Grid;
using UnicodeEngine.Sprites;
using Easy;
using System.Collections.Generic;


internal class PlayerMissile : Sprite
{
    //override public void OnHit(int damage) { this.HitPoints += damage; } // i don't think this is needed, does the same as the base class
    public PlayerMissile(UnicodeEngine.Grid.Point xy, UnicodeEngine.Grid.Trajectory t, char ascii) { constructor(xy, t, ascii); }
    public PlayerMissile(UnicodeEngine.Grid.Point xy, UnicodeEngine.Grid.Trajectory t) { constructor(xy, t, '|'); }

    public void constructor(UnicodeEngine.Grid.Point xy, UnicodeEngine.Grid.Trajectory t, char ascii)
    {
        this.Text = new[] { ascii };
        this.Trail = new UnicodeEngine.Grid.Trail(xy);
        this.Trajectory = t;
        this.Color = System.ConsoleColor.Red;
    }

}

internal class PlayerTorpedo : Sprite
{
    public PlayerTorpedo(UnicodeEngine.Grid.Point xy)
    {
        this.Text = new[] { '\x25b2' };
        this.Trail = new UnicodeEngine.Grid.Trail(xy);
        this.Trajectory = new Trajectory(-1, 0, Screen.Height / 2);
        this.Color = System.ConsoleColor.Red;

    }
}

internal class Player : Sprite
{

    public enum eFlightMode { Maneuver, Attack }
    eFlightMode FlightMode;
    double Run;
    public int DefaultHitPoints { get { return 5; } }

    public Swarm Missiles = new Swarm();
    public Swarm Torpedos = new Swarm();
    public int TorpedosLocked = 0;

    int MyMissileCapacity = 1;
    public int MissileCapacity { get { return this.MyMissileCapacity; } }

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
            this.Text = "\x00b7==\x00b7".ToCharArray();
            this.Run = 1.5;
        }
        else
        {
            this.FlightMode = eFlightMode.Attack;
            this.Text = ":><:".ToCharArray();
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
            UnicodeEngine.Sprites.Static.Swarms.Add(new UnicodeEngine.Fx.Explosion(new string(UnicodeWars.xShield, 5).ToCharArray(), this.XY.Clone(this.Width / 2, 0), 0, 3, 1, true, false, true, true));
        }
        else if (hiteffect < 0)
        {
            if (this.HitPoints > 0) { UnicodeEngine.Sprites.Static.Swarms.Add(new UnicodeEngine.Fx.Explosion(new string(UnicodeWars.xHit, this.Width).ToCharArray(), this.XY, this.Width, 3, 1, true, false, true, true)); }
            else { UnicodeEngine.Sprites.Static.Swarms.Add(new UnicodeEngine.Fx.Explosion(new string(UnicodeWars.xHit, 50).ToCharArray(), this.XY, this.Width, 20, 2, true, false, true, true)); }
        }
        else
        {
            UnicodeEngine.Sprites.Static.Swarms.Add(new UnicodeEngine.Fx.Explosion(new string('+', 3).ToCharArray(), this.XY.Clone(this.Width / 2, 0), 0, 3, 1, true, false, true, true));
        }

    }

    public Player()
    {
        SetFlightMode(eFlightMode.Maneuver);
        this.FlyZone = new FlyZoneClass(0, 1, 0, 0, FlyZoneClass.eEdgeMode.Stop);
        this.Trail = new Trail(new Point(Screen.Width / 2 - this.Width / 2, 0));
        this.HitPoints = this.DefaultHitPoints;
        this.HitEffect = -1;
        this.DropIn(.5);
        this.Color = System.ConsoleColor.DarkYellow;
    }

    public void FireTorpedo()
    {
        if (this.TorpedosLocked > 0)
        {
            this.TorpedosLocked -= 1;
            this.Torpedos.Add(new PlayerTorpedo(this.XY.Clone(this.Width / 2, 0)));
        }
    }
    public void Fire()
    {
        if (this.FlightMode == eFlightMode.Maneuver)
        {
            if (this.Missiles.Items.Count < this.MyMissileCapacity)
            {
                PlayerMissile missile = new PlayerMissile(this.XY.Clone(this.Width / 2, 0), new Trajectory(-.66, 0, this.XY.dY + 1));
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
                    PlayerMissile missile = new PlayerMissile(new Point(x + i, this.XY.iY), new Trajectory(-1, 0, this.XY.dY + 1));
                    missile.HitPoints = 1;
                    this.Missiles.Items.Add(missile);
                }
            }

        }
    }

    public void DropIn(double FallRate)
    {
        this.Hide();
        this.Trajectory = new Trajectory(FallRate, 0);
        this.Trail.Add(new Point(this.XY.dX, Screen.TopEdge));
    }

    public void FireSpread()
    {
        for (double run = -2; run < 2; run += .2)
        {
            PlayerMissile missile = new PlayerMissile(this.XY.Clone(this.Width / 2, 0), new Trajectory(-1, run, Screen.Height * .75));
            missile.HitPoints = 1;
            this.Missiles.Items.Add(missile);
        }
    }

    void TorpedoExplode(UnicodeEngine.Grid.Point xy)
    {
        for (double rise = -1; rise < 1; rise += .5)
        {

            for (double run = -2; run < 2; run += .5)
            {
                if (rise * run != 0)
                {
                    PlayerMissile missile = new PlayerMissile(xy, new Trajectory(rise, run, System.Math.Abs(rise * run*20)),UnicodeWars.xHit );
                    missile.HitPoints = 1;
                    this.Missiles.Items.Add(missile);
                }
            }
        }
    }



    public void FireAirStrike()
    {
        for (double x = Screen.LeftEdge; x < Screen.RightEdge; x += 5)
        {
            PlayerMissile missile = new PlayerMissile(new Point(x, 0), new Trajectory(1, 0, Screen.Height));
            missile.HitPoints = 1;
            this.Missiles.Items.Add(missile);
        }
    }


    override public void Activate()
    {
        Torpedos.Animate();
        Missiles.Animate();
        foreach (PlayerTorpedo t in Torpedos.Items) {
            if (!t.Alive) { this.TorpedoExplode(t.XY); }
        }
        if (!this.Alive && this.Missiles.Empty && this.Torpedos.Empty) { this.Active = false; }
    }

}