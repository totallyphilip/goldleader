using AsciiEngine;
using System;

public class Player : Sprite
{

    public SpriteField Missiles = new AsciiEngine.SpriteField();

    public int MaxMissiles = 5;

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
    }

}