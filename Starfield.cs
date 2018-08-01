using AsciiEngine;
using AsciiEngine.Grid;
using AsciiEngine.Sprites;
using Easy;
using System.Collections.Generic;

public class Starfield : Swarm
{
    bool InHyperspace = false;
    double Speed;
    double RowCoverageFactor;
    char Starlight
    {
        get
        {
            if (this.InHyperspace) { return '|'; }
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
        while (this.Count < this.MaxStars)
        {
            int x = Abacus.Random.Next(Screen.LeftEdge, Screen.RightEdge + 1);
            int y = Screen.TopEdge - 1;
            if (randomly) { y = Abacus.Random.Next(Screen.TopEdge - 1, Screen.Height); }
            Point xy = new Point(x, y);
            this.Add(new Sprite(new[] { this.Starlight }, xy, new Trajectory(this.Speed, 0, Screen.Height - y)));
        }

    }

    public void SetHyperspace(bool inhyperspace)
    {
        this.InHyperspace = inhyperspace;
        foreach (Sprite star in Items) { star.Ascii = new[] { this.Starlight }; }
    }
}



public class Galaxy
{
    List<Starfield> starfields = new List<Starfield>();

    public void Animate()
    {
        foreach (Starfield s in starfields) { s.Animate(); }
    }

    public void SetHyperspace(bool inhyperspace)
    {
        System.Console.Clear();
        foreach (Starfield s in starfields) { s.SetHyperspace(inhyperspace); }
    }

    public Galaxy()
    {
        starfields.Add(new Starfield(.1, .75)); // slow
        starfields.Add(new Starfield(1, .2)); // fast
    }
}