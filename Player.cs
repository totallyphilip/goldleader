using AsciiEngine;
using System;

public class Player
{



    int _X;
    int _Y;
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
        this._X = Screen.Width / 2 - this.Width / 2;
    }
    void Hide()
    {
        Screen.TryWrite(this._X, this._Y, new String(' ', this._Ascii.Length));
    }

    public void Animate()
    {
        this.Hide();
        this._Y = Screen.BottomEdge;
        if (this._X < Screen.LeftEdge) { this._Direction = 1; }
        if (this._X + this.Width > Screen.RightEdge) { this._Direction = -1; }
        this._X += this._Direction;
        Screen.TryWrite(this._X, this._Y, this._Ascii);  // show it

        this._missiles.Animate();
    }

    public void AddMissile()
    {
        this._missiles.Sprites.Add(new Sprite('|', this._X + this.Width / 2, this._Y, 0, -1, this._Y));
    }

}