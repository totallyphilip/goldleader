using AsciiEngine;
using AsciiEngine.Coordinates;
using AsciiEngine.Sprites;
using System;
using System.Collections.Generic;

public class TieFighterGame
{

    public static int Score;
    public void TryPlay()
    {

        bool LinuxDevMode = false;

        int oldwidth = Console.WindowWidth;
        int oldheight = Console.WindowHeight;

        if (LinuxDevMode || Screen.TrySetSize(45, 35))
        {
            Console.CursorVisible = false;
            Easy.Keyboard.EatKeys();
            this.Looper();
            Console.CursorVisible = true;
        }
        else
        {
            Console.Write("something went horribly wrong");
            Console.ReadKey();
        }

        Screen.TrySetSize(oldwidth, oldheight, false);

    }

    public void Demo()
    {
        Console.Clear();
        Swarm badguys = new Swarm();

        List<string> Messages = new List<string>();

        Messages.Add("A S C I I   W A R S");
        Messages.Add("");
        Messages.Add("- Enemies -");

        foreach (BadGuy.eBadGuyType shiptype in (BadGuy.eBadGuyType[])System.Enum.GetValues(typeof(BadGuy.eBadGuyType)))
        {
            BadGuy bg = new BadGuy(shiptype);
            badguys.Items.Add(bg);
            Messages.Add(new string(bg.Ascii) + " " + Enum.GetName(typeof(BadGuy.eBadGuyType), shiptype) + " (" + bg.HP + " HP)");
        }
        Messages.Add("");

        Messages.Add("- Scoring -");
        Messages.Add("Hit = 1 X Altitude Bonus");
        Messages.Add("Kill = 2 X Altitude Bonus");
        Messages.Add("");
        Messages.Add("- Controls -");
        Messages.Add("Esc = Quit");
        Messages.Add("Up/Down = Faster/Slower");
        Messages.Add("Tab = Hyperdrive");
        Messages.Add("Space = Fire");
        Messages.Add("Left/Right = Move");


        MessageScroller Scroller = new MessageScroller();
        do
        {
            if (Scroller.Empty)
            {
                foreach (string s in Messages)
                {
                    Scroller.AddMessage(s);
                }
            }
            badguys.Animate();
            Scroller.Animate();
            Easy.Clock.FpsThrottle(8);
        } while (!Console.KeyAvailable);

    }

    void Looper()
    {
        do
        {
            Demo();
        } while (Play());
    }

    public bool Play()
    {
        Demo();
        Console.Clear();
        MessageScroller Scroller = new MessageScroller();

        Score = 0;
        bool FirstBlood = false;

        // Star fields
        List<Starfield> starfields = new List<Starfield>();
        starfields.Add(new Starfield(.1, .75)); // slow
        starfields.Add(new Starfield(1, .2)); // fast

        // Make baddies
        BadGuyField badguys = new BadGuyField();

        // The player
        Player player = new Player();
        player.HP = 3;

        // keyboard buffer
        List<ConsoleKeyInfo> keybuffer = new List<ConsoleKeyInfo>();

        // Main loop
        int MasterFPS = 8;
        int FPS = MasterFPS;
        bool UserQuit = false;
        bool debug = false;
        int SkipFrames = 0;

        do
        {

            foreach (Starfield starfield in starfields) { starfield.Animate(); }
            if (player.Alive) { player.Animate(); }
            if (player.Active) { player.DoActivities(); }

            badguys.CheckCollisions(player.Missiles);
            badguys.Animate();
            badguys.CheckCollisions(player.Missiles);

            foreach (BadGuy bg in badguys.Items)
            {
                bg.Missiles.CheckCollision(player);
            }

            if (!FirstBlood && !badguys.Alive)
            {
                FirstBlood = true;
                Scroller.AddMessage("Great, kid!");
                Scroller.AddMessage("Don't get cocky.");
            }

            //if (boom != null) { boom.Animate(); }
            System.Console.Title = "Score: " + TieFighterGame.Score;

            Scroller.Animate();

            if (debug)
            {
                int x = Screen.LeftEdge;
                int y = Screen.TopEdge;
                Screen.TryWrite(x, y, "[fps: " + FPS + " ships: " + badguys.Items.Count + ']');
            }

            if (SkipFrames < 1) { FPS = MasterFPS; }
            else { FPS = int.MaxValue; SkipFrames--; }
            Easy.Clock.FpsThrottle(FPS);


            // wipe the keyboard buffer if a priority key is pressed
            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo k = Console.ReadKey(true);
                if (
                    k.Key == ConsoleKey.LeftArrow
                        || k.Key == ConsoleKey.RightArrow
                        || k.Key == ConsoleKey.Escape
                        || k.Key == ConsoleKey.UpArrow
                        || k.Key == ConsoleKey.DownArrow
                 )
                {
                    keybuffer = new List<ConsoleKeyInfo>(keybuffer.RemoveAll(x => x.Key == ConsoleKey.RightArrow || x.Key == ConsoleKey.LeftArrow));
                    keybuffer.Insert(0, k);
                }
                else
                {
                    keybuffer.Add(k);
                }
            }

            if (keybuffer.Count > 0)
            {

                ConsoleKeyInfo k = keybuffer[0];
                keybuffer.Remove(k);
                switch (k.Key)
                {
                    case ConsoleKey.Tab:
                        SkipFrames = 20;
                        break;
                    case ConsoleKey.UpArrow:
                        MasterFPS++;
                        break;
                    case ConsoleKey.DownArrow:
                        if (MasterFPS - 1 > 0) { MasterFPS--; }
                        break;
                    case ConsoleKey.LeftArrow:
                        player.Trajectory.Run = -1;
                        break;
                    case ConsoleKey.RightArrow:
                        player.Trajectory.Run = 1;
                        break;
                    case ConsoleKey.Spacebar:
                        player.Fire();
                        break;
                    case ConsoleKey.Escape:
                        UserQuit = true;
                        break;
                    case ConsoleKey.D:
                        debug = !debug;
                        break;
                }

            }

        } while (!UserQuit && player.Active);
        return !UserQuit;
    }
}