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

    void AddMessage(string s, ref Swarm messages)
    {
        messages.Items.Add(new Sprite(s.ToCharArray(), new Point(Screen.Width / 2 - s.Length / 2, Screen.Height), new Trajectory(-.5, 0, Screen.Height / 2)));
    }

    public void Demo()
    {
        Console.Clear();
        Swarm Messages = new Swarm();
        Swarm badguys = new Swarm();
        List<string> messagetext = new List<string>();

        int m = 0;

        messagetext.Add("A S C I I   W A R S");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("- Enemies -");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");

        foreach (BadGuy.eBadGuyType shiptype in (BadGuy.eBadGuyType[])System.Enum.GetValues(typeof(BadGuy.eBadGuyType)))
        {
            BadGuy bg = new BadGuy(shiptype);
            badguys.Items.Add(bg);
            messagetext.Add(new string(bg.Ascii) + " " + Enum.GetName(typeof(BadGuy.eBadGuyType), shiptype) + " (" + bg.HP + " HP)");
            messagetext.Add("");
            messagetext.Add("");
            messagetext.Add("");


        }

        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("- Scoring -");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("Hit = 1 X Altitude Bonus");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("Kill = 2 X Altitude Bonus");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("- Controls -");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("Esc = Quit");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("Up/Down = Faster/Slower");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("Tab = Hyperdrive");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("Space = Fire");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("Left/Right = Move");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");
        messagetext.Add("");

        do
        {
            badguys.Animate();
            Messages.Animate();
            AddMessage(messagetext[m], ref Messages);
            m++;
            if (m == messagetext.Count) { m = 0; }

            Easy.Clock.FpsThrottle(4);

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

        Score = 0;

        // Star fields
        List<Starfield> starfields = new List<Starfield>();
        starfields.Add(new Starfield(.1, .75)); // slow
        starfields.Add(new Starfield(1, .2)); // fast

        // Make baddies
        BadGuyField badguys = new BadGuyField();

        // The player
        Player player = new Player();

        // keyboard buffer
        List<ConsoleKeyInfo> keybuffer = new List<ConsoleKeyInfo>();

        // Main loop
        int MasterFPS = 8;
        int FPS = MasterFPS;
        bool UserQuit = false;
        bool debug = false;
        int SkipFrames = 0;

        // testing
        AsciiEngine.Fx.Explosion boom = null;

        do
        {

            foreach (Starfield starfield in starfields) { starfield.Animate(); }
            if (player.Alive) { player.Animate(); }
            if (player.Active) { player.DoActivities(); }
            player.CheckBadGuyHits(badguys);
            player.CheckHitByBadGuys(badguys);
            badguys.Animate();
            if (boom != null) { boom.Animate(); }
            System.Console.Title = "Score: " + TieFighterGame.Score;

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
                    case ConsoleKey.T:

                        break;
                }

            }

        } while (!UserQuit && player.Active);
        return !UserQuit;
    }
}