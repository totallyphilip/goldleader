public class Starfield
{
    AsciiEngine.SpriteField stars = new AsciiEngine.SpriteField();
    double _speed;
    double _rowcoveragepct;

    int MaxStars
    {
        get { return AsciiEngine.RoundNumber(AsciiEngine.Height * this._rowcoveragepct); }
    }

    public Starfield(double speed, double coverage)
    {
        this._speed = speed;
        this._rowcoveragepct = coverage;

        // spawn initial stars

        System.Random r = new System.Random();
        while (stars.Sprites.Count < this.MaxStars)
        {
            int y = r.Next(AsciiEngine.TopEdge - 1, AsciiEngine.Height);
            this.stars.Sprites.Add(new AsciiEngine.Sprite('.', r.Next(AsciiEngine.LeftEdge, AsciiEngine.RightEdge), y, 0, this._speed, AsciiEngine.Height - y));

        }

    }

    public void Animate()
    {
        while (stars.Sprites.Count < this.MaxStars)
        {
            System.Random r = new System.Random();
            this.stars.Sprites.Add(new AsciiEngine.Sprite('.', r.Next(AsciiEngine.LeftEdge, AsciiEngine.RightEdge), AsciiEngine.TopEdge - 1, 0, this._speed, AsciiEngine.Height));
        }

        this.stars.Animate();
    }

}
