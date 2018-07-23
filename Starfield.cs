using AsciiEngine;
using Easy;
public class Starfield
{
    SpriteField stars = new SpriteField();
    double _speed;
    double _rowcoveragepct;

    int MaxStars
    {
        get { return Numbers.Round(Screen.Height * this._rowcoveragepct); }
    }

    public Starfield(double speed, double coverage)
    {
        this._speed = speed;
        this._rowcoveragepct = coverage;

        // spawn initial stars

        while (stars.Sprites.Count < this.MaxStars)
        {
            int x = Numbers.Random.Next(Screen.LeftEdge, Screen.RightEdge);
            int y = Numbers.Random.Next(Screen.TopEdge - 1, Screen.Height);
            this.stars.Sprites.Add(new AsciiEngine.Sprite(new[] { '.' }, new Screen.Coordinate(x, y), new Screen.Trajectory(0, this._speed, Screen.Height - y)));
        }

    }

    public void Animate()
    {
        while (stars.Sprites.Count < this.MaxStars)
        {
            Screen.Coordinate xy = new Screen.Coordinate(Numbers.Random.Next(Screen.LeftEdge, Screen.RightEdge), Screen.TopEdge - 1);
            this.stars.Sprites.Add(new AsciiEngine.Sprite(new[] { '.' }, xy, new Screen.Trajectory(0, this._speed, Screen.Height)));
        }

        this.stars.Animate();
    }

}
