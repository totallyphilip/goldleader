using AsciiEngine;
using Easy;
public class Starfield
{
    AsciiEngine.SpriteField stars = new AsciiEngine.SpriteField();
    double _speed;
    double _rowcoveragepct;

    int MaxStars
    {
        get { return Mathy.Round(Screen.Height * this._rowcoveragepct); }
    }

    public Starfield(double speed, double coverage)
    {
        this._speed = speed;
        this._rowcoveragepct = coverage;

        // spawn initial stars

        while (stars.Sprites.Count < this.MaxStars)
        {
            int y = Mathy.Random.Next(Screen.TopEdge - 1, Screen.Height);
            this.stars.Sprites.Add(new AsciiEngine.Sprite('.', Mathy.Random.Next(Screen.LeftEdge, Screen.RightEdge), y, 0, this._speed, Screen.Height - y));

        }

    }

    public void Animate()
    {
        while (stars.Sprites.Count < this.MaxStars)
        {
            this.stars.Sprites.Add(new AsciiEngine.Sprite('.', Mathy.Random.Next(Screen.LeftEdge, Screen.RightEdge), Screen.TopEdge - 1, 0, this._speed, Screen.Height));
        }

        this.stars.Animate();
    }

}
