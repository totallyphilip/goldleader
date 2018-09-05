using AsciiEngine;
using AsciiEngine.Grid;
using AsciiEngine.Sprites;
using Easy;
using System;
public class Zombie
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
        People people = new People(100);
        Complex blockers = new Complex();
        blockers.Add(people);
        people.BlockingSwarms = blockers;
        people.Refresh(); // make sure everybody draws once since they might be initially blocked from moving
        bool gtfo = false;
        do
        {
            people.Animate();
            //Easy.Clock.FpsThrottle(6);
            if (Console.KeyAvailable)
            {
                Point newxy = new Point();
                ConsoleKeyInfo k = Console.ReadKey(true);
                if (k.Key == ConsoleKey.Home) { newxy = new Point(Screen.LeftEdge,Screen.TopEdge); }
                else if (k.Key == ConsoleKey.UpArrow) { newxy = new Point(Screen.Width/2, Screen.TopEdge); }
                else if (k.Key == ConsoleKey.PageUp) { newxy = new Point(Screen.RightEdge, Screen.TopEdge); }
                else if (k.Key == ConsoleKey.LeftArrow) { newxy = new Point(Screen.LeftEdge, Screen.Height/2); }
                else if (k.Key == ConsoleKey.Clear) { newxy = new Point(Screen.Width/2, Screen.Height/2); }
                else if (k.Key == ConsoleKey.RightArrow) { newxy = new Point(Screen.RightEdge, Screen.Height/2); }
                else if (k.Key == ConsoleKey.End) { newxy = new Point(Screen.LeftEdge, Screen.BottomEdge); }
                else if (k.Key == ConsoleKey.DownArrow) { newxy = new Point(Screen.Width/2, Screen.BottomEdge); }
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

    public Point Target;
    double Speed = Abacus.Random.NextDouble() + .1;

    override public void Activate()
    {

        if (this.Alive)
        {
            this.GetNewTrajectory();
        }


        if (!this.Alive) { this.Active = false; }

    }

    void GetNewTrajectory()
    {
        Point mynewxy;
//        if (Abacus.Random.NextDouble() < .25) { mynewxy = this.Target.Clone(Abacus.Random.Next(20) - 10, Abacus.Random.Next(20) - 10); }
        if (Abacus.Random.NextDouble() < .05) { mynewxy = new Point(Abacus.Random.Next(Screen.Width*2)-Screen.Width, Abacus.Random.Next(Screen.Height*2)-Screen.Height ) ; }
        else { mynewxy = this.Target.Clone(); }
        Abacus.Slope slope = Abacus.SlopeFrom(mynewxy, this.XY);
        //        this.Trajectory = new Trajectory(slope.Rise+(Abacus.Random.NextDouble()-.5)*this.Speed, slope.Run+ (Abacus.Random.NextDouble() - .5)*this.Speed);
                this.Trajectory = new Trajectory(slope.Rise*this.Speed, slope.Run*this.Speed);
    }

    public Person(Point xy)
    {
        this.FlyZone.EdgeMode = Sprite.FlyZoneClass.eEdgeMode.Stop;
        constructor(new[] { (Abacus.RandomTrue ? Symbol.FaceBlack : Symbol.FaceWhite) }, xy, new Trajectory(0, 0), ConsoleColor.White);
        Target = xy;
    }
}
internal class People : Swarm
{

    public People(int count)
    {
        do
        {
            Point xy = new Point(Abacus.Random.Next(Screen.Width), Abacus.Random.Next(Screen.Height));
            if (!this.Items.Exists(x => x.XY.iX == xy.iX && x.XY.iY == xy.iY))
            {
                Person person = new Person(xy);
                person.Target = new Point(Screen.Width / 2, Screen.Height / 2);
                this.Add(person);
            }

        } while (this.Count < count);


    }



}