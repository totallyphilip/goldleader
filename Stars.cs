using UnicodeEngine;
using UnicodeEngine.Grid;
using UnicodeEngine.Sprites;
using Easy;
//using System.Collections.Generic;

internal class Starfield : Swarm
{
    bool InHyperspace = false;
    double Speed;
    System.ConsoleColor Color;
    double RowCoverageFactor;
    char Starlight { get { if (this.InHyperspace) { return '|'; } else { return '.'; } } } // 00b7 is center dot
    int MaxStars { get { return Abacus.Round(Screen.Height * this.RowCoverageFactor); } }

    public Starfield(double speed, double coverage, System.ConsoleColor color)
    {
        this.Speed = speed;
        this.RowCoverageFactor = coverage;
        this.Color = color;
        this.Spawn(true);
    }

    protected override void Spawn() { this.Spawn(false); }

    public void Spawn(bool randomly)
    {
        while (this.Count < this.MaxStars)
        {
            int x = Abacus.Random.Next(Screen.LeftEdge, Screen.RightEdge + 1);
            int y = Screen.TopEdge - (Abacus.Random.Next(5)); // randomly set stars back so the pattern doesn't repeat as stars fall off the bottom
            if (randomly) { y = Abacus.Random.Next(Screen.TopEdge - 1 , Screen.Height); }
            Point xy = new  Point(x, y);
            this.Add(new Sprite(new[] { this.Starlight }, xy, new Trajectory(this.Speed, 0, Screen.Height - y), this.Color));
        }

    }

    public void SetHyperspace(bool inhyperspace)
    {
        this.InHyperspace = inhyperspace;
        foreach (Sprite star in Items) { star.Text = new[] { this.Starlight }; }
    }
}



internal class Galaxy: Complex
{
    public void SetHyperspace(bool inhyperspace)
    {
        System.Console.Clear();
        foreach (Starfield s in this.Items) { s.SetHyperspace(inhyperspace); }
    }

    public Galaxy()
    {
        this.Add(new Starfield(.1, .75, System.ConsoleColor.DarkGray)); // slow
        this.Add(new Starfield(1, .2, System.ConsoleColor.White)); // fast
    }
}