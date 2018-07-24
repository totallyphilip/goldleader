using AsciiEngine;
using Easy;
public class Starfield : SpriteField
{
    double Speed;
    double RowCoverageFactor;
    int MaxStars { get { return Numbers.Round(Screen.Height * this.RowCoverageFactor); } }

    public Starfield(double speed, double coverage)
    {
        this.Speed = speed;
        this.RowCoverageFactor = coverage;
        this.Spawn(true);
    }

    public void Spawn() { this.Spawn(false); }

    public void Spawn(bool randomly)
    {
        while (this.Items.Count < this.MaxStars)
        {
            int x = Numbers.Random.Next(Screen.LeftEdge, Screen.RightEdge);
            int y = Screen.TopEdge - 1;
            if (randomly) { y = Numbers.Random.Next(Screen.TopEdge - 1, Screen.Height); }
            Screen.Coordinate xy = new Screen.Coordinate(x, y);
            this.Items.Add(new AsciiEngine.Sprite(new[] { '.' }, xy, new Screen.Trajectory(this.Speed, 0, Screen.Height - y)));
        }

    }

    protected override void AnimateOverride()
    {
        this.Spawn();
    }

}
