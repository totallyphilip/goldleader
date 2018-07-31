using AsciiEngine;
using AsciiEngine.Grid;
using AsciiEngine.Sprites;
using Easy;
using System.Collections.Generic;

public class Starfield : Swarm
{
    double Speed;
    double RowCoverageFactor;
    bool Hyperspace = false;
    char Starlight
    {
        get
        {
            if (this.Hyperspace) { return '|'; }
            else { return '.'; }
        }
    }
    int MaxStars { get { return Abacus.Round(Screen.Height * this.RowCoverageFactor); } }

    public Starfield(double speed, double coverage)
    {
        this.Speed = speed;
        this.RowCoverageFactor = coverage;
        this.Spawn(true);
    }

    protected override void Spawn() { this.Spawn(false); }

    public void Spawn(bool randomly)
    {
        while (this.Items.Count < this.MaxStars)
        {
            int x = Abacus.Random.Next(Screen.LeftEdge, Screen.RightEdge);
            int y = Screen.TopEdge - 1;
            if (randomly) { y = Abacus.Random.Next(Screen.TopEdge - 1, Screen.Height); }
            Point xy = new Point(x, y);
            this.Items.Add(new Sprite(new[] { this.Starlight }, xy, new Trajectory(this.Speed, 0, Screen.Height - y)));
        }

    }

    void RedefineStarlight(bool hyperspace)
    {
        this.Hyperspace = hyperspace;
        foreach (Sprite star in Items) { star.Ascii = new[] { this.Starlight }; }
    }

    public void EnterHyperspace() { RedefineStarlight(true); }

    public void ExitHyperspace() { RedefineStarlight(false); }

}



public class Galaxy
{
    List<Starfield> starfields = new List<Starfield>();

    public void Animate()
    {
        foreach (Starfield s in starfields) { s.Animate(); }
    }

    public void EnterHyperspace()
    {
        System.Console.Clear();
        foreach (Starfield s in starfields) { s.EnterHyperspace(); }
    }
    public void ExitHyperspace()
    {
        foreach (Starfield s in starfields) { s.ExitHyperspace(); }
    }

    public Galaxy()
    {
        starfields.Add(new Starfield(.1, .75)); // slow
        starfields.Add(new Starfield(1, .2)); // fast
    }
}