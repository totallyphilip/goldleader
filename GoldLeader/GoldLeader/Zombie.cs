using AsciiEngine;
using AsciiEngine.Fx;
using AsciiEngine.Grid;
using AsciiEngine.Sprites;
using Easy;
using System;
using System.Collections.Generic;

internal class Compass
{
    static public int North { get { return Screen.Height / 9; } }
    static public int South { get { return Screen.Height - North; } }
    static public int West { get { return Screen.Width / 9; } }
    static public int East { get { return Screen.Width - West; } }
    static public int Equator { get { return Screen.Height / 2; } }
    static public int Meridian { get { return Screen.Width / 2; } }
}

public class ZombieGame
{


    public void TryPlay()
    {

        // main settings
        AsciiEngine.Application.Title = "BRAINS";
        AsciiEngine.Application.ID = Guid.Parse("E52D66E9-25BC-44B6-8DAE-23CDBBA5CDA4");


        int oldwidth = Console.WindowWidth;
        int oldheight = Console.WindowHeight;
        Screen.TryInitializeScreen(80, 30);
        this.MainLoop();
        Screen.TryInitializeScreen(oldwidth, oldheight);
        Console.CursorVisible = true;
    }

    void MainLoop()
    {
        int Escaped = 0;
        bool GaveOver = false;
        int FrameCounter = 0;
        int FlashbangCounter = 0;
        int FlashbangWaitCycles = 30;

        Console.CursorVisible = false;
        Console.WriteLine("Set off flashbangs with numpad keys (numlock unnecessary).");
        Console.WriteLine("Hit escape to quit.");
        Console.Write("Press any key to begin.");
        Console.ReadKey();
        Console.Clear();
        Scroller scroller = new Scroller(1, Screen.Height / 2, .33, ConsoleColor.Red, ConsoleColor.DarkRed);
        Swarm tombstones = new Swarm();
        tombstones.AlwaysAlive = true;
        People people = new People(20, tombstones);
        Zombies zombies = new Zombies(5);
        Gates gates = new Gates(2);

        people.BlockingSwarms = new Complex(tombstones);
        people.BlockingSwarms.Add(zombies);
        people.BlockingSwarms.Add(people);
        zombies.BlockingSwarms = new Complex(tombstones);
        zombies.BlockingSwarms.Add(zombies);
        zombies.BlockingSwarms.Add(gates);

        scroller.NewLine(Textify.Fluffer("READY PLAYER ONE", " "));

        bool gtfo = false;
        do
        {
            Console.CursorVisible = false;
            FlashbangCounter++;
            FrameCounter++;
            if (FrameCounter > 100)
            {
                FrameCounter = 0;
                zombies.MakeMonsters(1);
            }

            Static.Sprites.Animate();
            Static.Swarms.Animate();
            tombstones.Animate();
            zombies.Animate();
            people.Animate();
            zombies.CheckCollisions(people);
            gates.Animate();
            gates.CheckCollisions(people);
            scroller.Animate();

            foreach (Person p in people.Items.FindAll(x => x.Alive))
            {
                if (p.Escaped)
                {
                    p.Terminate();
                    Escaped++;
                }
            }


            // hud
            string hud = " " + (FlashbangCounter < FlashbangWaitCycles ? new string(Symbol.ShadeLight, 5) : new string(Symbol.FullBlock, 5)) + " " + Escaped + "|" + people.Count.ToString() + " " + (FlashbangCounter < FlashbangWaitCycles ? new string(Symbol.ShadeLight, 5) : new string(Symbol.FullBlock, 5)) + " ";
            Point hudxy = new Point(Screen.Width / 2 - hud.Length / 2, Screen.BottomEdge);
            Console.ForegroundColor = ConsoleColor.White;
            Screen.TryWrite(hudxy, hud);

            Easy.Clock.FpsThrottle(6);
            if (Console.KeyAvailable)
            {
                Point newxy = new Point();
                ConsoleKeyInfo k = Console.ReadKey(true);
                if (k.Key == ConsoleKey.Home) { newxy = new Point(Compass.West, Compass.North); }
                else if (k.Key == ConsoleKey.UpArrow) { newxy = new Point(Compass.Meridian, Compass.North); }
                else if (k.Key == ConsoleKey.PageUp) { newxy = new Point(Compass.East, Compass.North); }
                else if (k.Key == ConsoleKey.LeftArrow) { newxy = new Point(Compass.West, Compass.Equator); }
                else if (k.Key == ConsoleKey.Clear) { newxy = new Point(Compass.Meridian, Compass.Equator); }
                else if (k.Key == ConsoleKey.RightArrow) { newxy = new Point(Compass.East, Compass.Equator); }
                else if (k.Key == ConsoleKey.End) { newxy = new Point(Compass.West, Compass.South); }
                else if (k.Key == ConsoleKey.DownArrow) { newxy = new Point(Compass.Meridian, Compass.South); }
                else if (k.Key == ConsoleKey.PageDown) { newxy = new Point(Compass.East, Compass.South); }
                else if (k.Key == ConsoleKey.Escape) { gtfo = true; }

                if (FlashbangCounter > FlashbangWaitCycles)
                {
                    FlashbangCounter = 0;

                    foreach (Person p in people.Items) { p.Target = newxy; p.Color = ConsoleColor.White; }
                    for (int i = 0; i < 5; i++)
                    {
                        int zombieindex = Abacus.Random.Next(zombies.Count);
                        zombies.Items[zombieindex].Target = newxy;
                        Static.Swarms.Add(new Explosion(new[] { Symbol.FullBlock, Symbol.SmallBlock }, newxy, 0, 4, 3, true, true, true, true, ConsoleColor.White));
                        Static.Sprites.Add(new Sprite(new[] { '!' }, zombies.Items[zombieindex].XY.Clone(0, -1), new Trajectory(-.33, 0, 2), ConsoleColor.Red));

                    }
                }
                Easy.Keyboard.EatKeys();
            }

            if (people.AliveCount < 1 && !GaveOver)
            {
                scroller.NewLine(Textify.Fluffer("GAME OVER", " "));
                GaveOver = true;
            }
        } while (!gtfo);
    }

}

#region " People "

internal class Person : Sprite
{
    People parent;

    public int FullHealth = 3;
    public bool Escaped = false;

    Swarm Corpses;

    public override void Activate() // add more complex code in inherited tasks if needed
    {
        if (this.Alive)
        {
            // stuff to do if alive
        }

        // stuff to do regardless


        // if person got to its destination, stop moving
        if (AsciiEngine.Grid.Point.SamePlace(this.XY, this.Target))
        {
            this.Target = null;
            this.Trajectory = new Trajectory(0, 0);
        }

        if (!this.Alive) { this.Active = false; } // set false when no more stuff to do
    }

    public Person(Point xy, Swarm corpses, People parent)
    {
        this.FlyZone.EdgeMode = Sprite.FlyZoneClass.eEdgeMode.Stop;
        this.FlyZone.BottomMargin = 1;
        constructor(new[] { Symbol.FaceSolid }, xy, new Trajectory(0, 0), ConsoleColor.White);
        this.Target = xy;
        this.SpeedFactor = Abacus.Random.NextDouble();
        if (this.SpeedFactor < .3) { this.SpeedFactor = .3; }
        this.HitPoints = this.FullHealth;
        this.Corpses = corpses;
        this.parent = parent;
    }

    override public void OnHit(int hiteffect)
    {
        if (hiteffect < 0)
        {
            this.HitPoints += hiteffect;
            this.Color = ConsoleColor.Red;
            if (this.Alive)
            {
                this.Target = new Point(Abacus.Random.Next(Screen.Width), Abacus.Random.Next(Screen.Height));
                //Static.Sprites.Add(new Sprite(new[] { '!' }, this.XY.Clone(0, -1), new Trajectory(-.33, 0, 1), ConsoleColor.Red));
            }
            else
            {
                this.Corpses.Add(new Sprite(new[] { Symbol.FaceOutline }, this.XY.Clone(), new Trajectory(0, 0), ConsoleColor.Gray));
            }
        }
        else if (hiteffect > 0)
        {
            this.Escaped = true;
            Static.Swarms.Add(new Explosion(new string(Symbol.DotCenter, 2).ToCharArray(), this.XY, 0, 5, 1.5, true, true, true, true, ConsoleColor.Cyan));
            parent.MakePeople(1, Corpses, parent);
        }

    }


}


internal class People : Swarm
{

    public void MakePeople(int count, Swarm corpses, People parent)
    {
        for (int i = 0; i < count; i++)
        {
            Point xy = new Point(Screen.LeftEdge + 1, Screen.Height / 2);
            Person person = new Person(xy, corpses, this);
            person.Target = new Point(Screen.Width / 2, Screen.Height / 2);
            this.Add(person);
        }
    }

    public People(int count, Swarm corpses)
    {
        this.MakePeople(count, corpses, this);
    }

}

#endregion

#region " Monsters "

internal class Zombie : Sprite
{

    public override void Activate() // add more complex code in inherited tasks if needed
    {
        if (this.Alive)
        {
            // stuff to do if alive
        }

        // stuff to do regardless


        // if zombie got to its random location, pick a new random location
        if (AsciiEngine.Grid.Point.SamePlace(this.XY, this.Target))
        {
            this.Target = new Point(Abacus.Random.Next(this.FlyZone.RightEdge), Abacus.Random.Next(this.FlyZone.BottomEdge));
        }

        if (!this.Alive) { this.Active = false; } // set false when no more stuff to do
    }

    public Zombie(Point xy)
    {
        this.FlyZone.EdgeMode = Sprite.FlyZoneClass.eEdgeMode.Stop;
        this.FlyZone.BottomMargin = 1;
        constructor(new[] { Symbol.CrossDiagonal }, xy, new Trajectory(0, 0), ConsoleColor.DarkYellow);
        this.Target = xy;
        this.SpeedFactor = .1;
    }
}

internal class Zombies : Swarm
{

    public void MakeMonsters(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Point xy = new Point(Screen.RightEdge - 1, Screen.Height / 2);

            Zombie zombie = new Zombie(xy);
            zombie.Target = xy;
            this.Add(zombie);
        }
    }

    public Zombies(int count)
    {
        this.MakeMonsters(count);
    }


}

#endregion

#region " Gates "
internal class Gate : Sprite
{
    int FrameCounter = 0;

    char[] GateText()
    {
        return (Symbol.BarVerticalRight + new string(Symbol.ShadeLight, this.HitPoints) + Symbol.BarVerticalLeft + ' ').ToCharArray();
    }

    void GoBoom()
    {
        Static.Swarms.Add(new Explosion(new string(Symbol.DotCenter, 20).ToCharArray(), this.XY, 0, 5, 1.5, true, true, true, true, ConsoleColor.Yellow));
    }

    public override void Activate() // add more complex code in inherited tasks if needed
    {
        if (this.Alive)
        {
            // stuff to do if alive
            this.Text = this.GateText();
            this.Trajectory = new Trajectory(0, 0);
            FrameCounter++;
            if (FrameCounter > 10) { this.OnHit(-1); this.FrameCounter = 0; }
        }

        // stuff to do regardless

        if (!this.Alive) { this.Active = false; } // set false when no more stuff to do
    }

    public override void OnHit(int hiteffect)
    {
        this.Hide();
        this.HitPoints += hiteffect;
        this.Trail.Add(this.XY.Clone(.5, 0));
        this.Text = this.GateText();
        this.Refresh();
        if (!this.Alive)
        {
            this.GoBoom();
        }
    }

    public Gate()
    {
        this.HitPoints = 20;
        this.Text = this.GateText();
        this.Trail = new Trail(new Point(Abacus.Random.Next(Compass.West, Compass.East - this.Width), Abacus.Random.Next(Compass.North, Compass.South)));
        this.Trajectory = new Trajectory(0, 0);
        this.OriginalTrajectory = this.Trajectory.Clone();
        this.HitEffect = 1;
        this.Color = ConsoleColor.Blue;
    }
}

internal class Gates : Swarm
{

    public int Maximum;

    protected override void Spawn()
    {

        while (this.Count < this.Maximum)
        {
            this.Items.Add(new Gate());
        }


    }

    public Gates(int max)
    {
        this.Maximum = max;
    }
}

#endregion