using AsciiEngine;
using System;

public class Player : Sprite
{

    public SpriteField Missiles = new SpriteField();
    public SpriteField Messages = new SpriteField();

    public int MaxMissiles = 1;

    public Player()
    {
        this.Ascii = ":><:".ToCharArray();
        this.FlyZone = new FlyZoneClass(0, 0, 0, 0, FlyZoneClass.eEdgeMode.Stop);
        this.Trajectory = new Screen.Trajectory(0, 0);
        this.Trail = new Screen.CoordinateHistory(new Screen.Coordinate(Screen.Width / 2 - this.Width / 2, Screen.BottomEdge));
    }

    public void Fire()
    {
        if (this.Missiles.Items.Count < this.MaxMissiles)
        {
            this.Missiles.Items.Add(new Sprite(new[] { '|' }, new Screen.Coordinate(this.XY.X + this.Width / 2, this.XY.Y), new Screen.Trajectory(-1, 0, this.XY.Y)));
        }
    }

    public void AnimateMissiles()
    {
        Missiles.Animate();
        Messages.Animate();
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
                }

            }
        }
    }

}