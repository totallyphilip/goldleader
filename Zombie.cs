using AsciiEngine;
using AsciiEngine.Fx;
using AsciiEngine.Grid;
using AsciiEngine.Sprites;
using Easy;
using System;
using System.Collections.Generic;
public class ZombieGame
{

    public void TryPlay()
    {

        // main settings
        AsciiEngine.Application.Title = "BRAINS";
        Leaderboard.SqlConnectionString = "user id=dbTest;password=baMw$CAQ5hnlxjCTYJ0YP;server=sql01\\dev01;Trusted_Connection=no;database=PwrightSandbox;connection timeout=5";
        AsciiEngine.Application.ID = Guid.Parse("E52D66E9-25BC-44B6-8DAE-23CDBBA5CDA4");


        int oldwidth = Console.WindowWidth;
        int oldheight = Console.WindowHeight;
        Screen.TryInitializeScreen(80, 30, false);
        this.MainLoop();
        Screen.TryInitializeScreen(oldwidth, oldheight, false);
        Console.CursorVisible = true;
    }

    void MainLoop()
    {

        Console.CursorVisible = false;
        Console.WriteLine("Use numpad keys to move. NumLock is NOT necessary.");
        Console.WriteLine("Use spacebar to kill 10 people.");
        Console.WriteLine("Use escape to quit.");
        Console.Write("Press any key to begin");
        Console.ReadKey();
        Console.Clear();
        People people = new People(111);
        Zombies zombies = new Zombies(50);
        Complex everybody = new Complex();
        everybody.Items.Add(people);
        everybody.Items.Add(zombies);
        people.BlockingSwarms = everybody;
        zombies.BlockingSwarms = new Complex(zombies);
        bool gtfo = false;
        do
        {
            Console.CursorVisible = false;
            everybody.Animate();
            zombies.CheckCollisions(people);
            AsciiEngine.Sprites.Static.Swarms.Animate();

            // hud
            string survivors = " " + people.Count.ToString() + " ";
            Point hudxy = new Point(Screen.Width / 2 - survivors.Length / 2, Screen.BottomEdge);
            Console.ForegroundColor = ConsoleColor.White;
            Screen.TryWrite(hudxy, survivors);

            Easy.Clock.FpsThrottle(6);
            if (Console.KeyAvailable)
            {
                Point newxy = new Point();
                ConsoleKeyInfo k = Console.ReadKey(true);
                if (k.Key == ConsoleKey.Home) { newxy = new Point(Screen.LeftEdge, Screen.TopEdge); }
                else if (k.Key == ConsoleKey.UpArrow) { newxy = new Point(Screen.Width / 2, Screen.TopEdge); }
                else if (k.Key == ConsoleKey.PageUp) { newxy = new Point(Screen.RightEdge, Screen.TopEdge); }
                else if (k.Key == ConsoleKey.LeftArrow) { newxy = new Point(Screen.LeftEdge, Screen.Height / 2); }
                else if (k.Key == ConsoleKey.Clear) { newxy = new Point(Screen.Width / 2, Screen.Height / 2); }
                else if (k.Key == ConsoleKey.RightArrow) { newxy = new Point(Screen.RightEdge, Screen.Height / 2); }
                else if (k.Key == ConsoleKey.End) { newxy = new Point(Screen.LeftEdge, Screen.BottomEdge); }
                else if (k.Key == ConsoleKey.DownArrow) { newxy = new Point(Screen.Width / 2, Screen.BottomEdge); }
                else if (k.Key == ConsoleKey.PageDown) { newxy = new Point(Screen.RightEdge, Screen.BottomEdge); }
                else if (k.Key == ConsoleKey.Escape) { gtfo = true; }
                else if (k.Key == ConsoleKey.Spacebar) { newxy = people.Items[0].XY; for (int i = 0; i < 10; i++) { people.Items[i].Terminate(); } }
                foreach (Person p in people.Items)
                {
                    p.Target = newxy;
                }
                Easy.Keyboard.EatKeys();
            }
        } while (!gtfo);
    }

}



internal class Person : Sprite
{

    public int FullHealth = 3;
    public Person(Point xy)
    {
        this.FlyZone.EdgeMode = Sprite.FlyZoneClass.eEdgeMode.Stop;
        this.FlyZone.BottomMargin = 1;
            constructor(new[] {  Symbol.FaceBlack }, xy, new Trajectory(0, 0), ConsoleColor.White);
        this.Target = xy;
        this.SpeedFactor = Abacus.Random.NextDouble();
        if (this.SpeedFactor < .3) { this.SpeedFactor = .3; }
        this.HitPoints = this.FullHealth;

    }


    override public void OnHit(int damage)
    {
        this.HitPoints += damage;
        this.Color = ConsoleColor.Red;
        if (this.Alive)
        {
            Explosion splatter = new Explosion(new string(Symbol.DotCenter, this.FullHealth - this.HitPoints).ToCharArray(), this.XY, this.Width, 1.5, 1, true, true, true, true, ConsoleColor.Red);
            AsciiEngine.Sprites.Static.Swarms.Add(splatter);
            this.Target = new Point(Abacus.Random.Next(Screen.Width), Abacus.Random.Next(Screen.Height));
        }
        else
        {
            Explosion splatter = new Explosion(new string('x', this.FullHealth - this.HitPoints).ToCharArray(), this.XY, this.Width, 4, 1, true, true, true, true, ConsoleColor.Red);
            AsciiEngine.Sprites.Static.Swarms.Add(splatter);
        }

    }


}

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
            this.Target = new Point(Abacus.Random.Next(Screen.Width), Abacus.Random.Next(Screen.Height));
        }

        if (!this.Alive) { this.Active = false; } // set false when no more stuff to do
    }

    public Zombie(Point xy)
    {
        this.FlyZone.EdgeMode = Sprite.FlyZoneClass.eEdgeMode.Stop;
        this.FlyZone.BottomMargin = 1;
        constructor(new[] { Symbol.FaceWhite }, xy, new Trajectory(0, 0), ConsoleColor.Green);
        this.Target = xy;
        this.SpeedFactor = .1;
    }
}


internal class People : Swarm
{
    public People(int count)
    {
        do
        {
            // Point xy = new Point(Abacus.Random.Next(Screen.Width - 1), Abacus.Random.Next(Screen.Height - 1));
            Point xy = new Point(Screen.Width - 1, 0);
            {
                Person person = new Person(xy);
                person.Target = new Point(Screen.Width / 2, Screen.Height / 2);
                this.Add(person);
            }

        } while (this.Count < count);

    }

}

internal class Zombies : Swarm
{
    public Zombies(int count)
    {
        do
        {
            //            Point xy = new Point(Abacus.Random.Next(Screen.Width - 1), Abacus.Random.Next(Screen.Height - 1));
                        Point xy = new Point(0,0);
            {
                Zombie zombie = new Zombie(xy);
                zombie.Target = xy;
                this.Add(zombie);
            }

        } while (this.Count < count);

    }

}