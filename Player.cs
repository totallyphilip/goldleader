using AsciiEngine;
using System;

public class Player
{


    public Screen.Coordinate xy;
    int _Direction;
    AsciiEngine.SpriteField _missiles = new AsciiEngine.SpriteField();
    string _Ascii = ":><:";

    int Width { get { return this._Ascii.Length; } }

    public AsciiEngine.SpriteField Missiles { get { return this._missiles; } }

    public int Direction
    {
        get { return this._Direction; }
        set { this._Direction = value; }
    }
    public Player()
    {
        this.xy = new Screen.Coordinate(Screen.Width / 2 - this.Width / 2, Screen.BottomEdge);
    }
    void Hide()
    {
        Screen.TryWrite(xy, new String(' ', this._Ascii.Length));
    }

    public void Animate()
    {
        this.Hide();
        if (this.xy.X < Screen.LeftEdge) { this._Direction = 1; }
        if (this.xy.X + this.Width > Screen.RightEdge) { this._Direction = -1; }
        ;
        this.xy.X += this._Direction;
        this.xy.Y = Screen.BottomEdge;

        Screen.TryWrite(this.xy, this._Ascii);  // show it

        this._missiles.Animate();
    }

    public void AddMissile()
    {
        this._missiles.Items.Add(new Sprite(new[] { '|' }, new Screen.Coordinate(this.xy.X + this.Width / 2, this.xy.Y), new Screen.Trajectory(-1, 0, this.xy.Y)));
    }

}