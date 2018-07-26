using AsciiEngine;
using AsciiEngine.Coordinates;
using AsciiEngine.Sprites;
using Easy;

public class Player : Sprite
{

    public Swarm Missiles = new Swarm();
    public Swarm Messages = new Swarm();
    public int Score = 0;

    public int MaxMissiles = 1;
    Swarm Debris = new Swarm();

    public Player()
    {
        this.Ascii = ":><:".ToCharArray();
        this.FlyZone = new FlyZoneClass(0, 0, 0, 0, FlyZoneClass.eEdgeMode.Stop);
        this.Trajectory = new Trajectory(0, 0);
        this.Trail = new Trail(new Coordinate(Screen.Width / 2 - this.Width / 2, Screen.BottomEdge));
        this.HP = 1;
    }

    public void BigExplosion()
    {
        foreach (char c in "\x00d7*#-".ToCharArray())
        {
            for (int i = 0; i < 10; i++)
            {
                double Rise = -2 * (Numbers.Random.NextDouble() + .1);
                double Run = 2 * Numbers.Random.NextDouble();
                if (Numbers.Random.NextDouble() < .5) { Run *= -1; }
                Debris.Items.Add(new Sprite(new[] { c }, new Coordinate(this.XY.X + this.Width / 2, this.XY.Y), new Trajectory(Rise, Run, 20)));
            }
        }
    }

    public void Fire()
    {
        if (this.Missiles.Items.Count < this.MaxMissiles)
        {
            this.Missiles.Items.Add(new Sprite(new[] { '|' }, new Coordinate(this.XY.X + this.Width / 2, this.XY.Y), new Trajectory(-1, 0, this.XY.Y)));
        }
    }

    public void CheckBadGuyHits(BadGuyField badguys)
    {
        foreach (Sprite missile in this.Missiles.Items.FindAll(x => x.Alive))
        {
            foreach (BadGuy badguy in badguys.Items.FindAll(x => x.Alive))
            {
                if (badguy.Hit(missile.XY))
                {
                    missile.Terminate();
                    badguy.MakeDebris();
                    this.Score++;
                    System.Console.Title = "Score: " + this.Score;
                }

            }
        }

        if (this.MaxMissiles < badguys.MaxBadGuys / 3 && this.Alive)
        {
            this.MaxMissiles++;
            this.Messages.Items.Add(new Sprite("Extra Missile".ToCharArray(), this.XY, new Trajectory(-1, 0, Screen.Height / 2)));
        }

    }

    override public void DoActivities()
    {
        Missiles.Animate();
        Messages.Animate();
        Debris.Animate();

        if (!this.Alive && this.Debris.Items.Count < 1 && this.Missiles.Items.Count < 1) { this.Active = false; }
    }

    public void CheckHitByBadGuys(BadGuyField badguys)
    {
        foreach (BadGuy badguy in badguys.Items)
        {
            foreach (Sprite missile in badguy.Missiles.Items)
            {
                if (this.Hit(missile.XY))
                {
                    missile.Terminate();
                    this.BigExplosion();
                }

            }
        }
    }


}