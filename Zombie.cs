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
        People people = new People(70);
        people.Refresh(); // make sure everybody draws once since they might be initially blocked from moving
        do
        {
            people.CheckBlocked();
            people.Animate();
            Easy.Clock.FpsThrottle(1);
        } while (!Console.KeyAvailable);
    }

}

internal class People : Swarm
{

    public People(int count)
    {
        do
        {
            Point xy = new Point(Abacus.Random.Next(10), Abacus.Random.Next(Screen.Height / 2 - 5, Screen.Height / 2 + 5));
            if (!this.Items.Exists(x => x.XY.iX == xy.iX && x.XY.iY == xy.iY))
            {
                Sprite person = new Sprite(new[] { (Abacus.RandomTrue ? Symbol.FaceBlack : Symbol.FaceWhite) }, xy, new Trajectory(0, 0), ConsoleColor.White);
                person.Trajectory = new Trajectory(-.1, 1);
                this.Add(person);
            }

        } while (this.Count < count);


    }



}