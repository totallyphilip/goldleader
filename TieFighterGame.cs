using AsciiEngine;
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
            Screen.Countdown(5);
            Easy.Keyboard.EatKeys();
            this.Play();
            Console.CursorVisible = true;
        }
        else
        {
            Console.Write("something went horribly wrong");
            Console.ReadKey();
        }

        Screen.TrySetSize(oldwidth, oldheight, false);

    }

    public void Play()
    {

        Console.Clear();

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

        if (!UserQuit) { Screen.Countdown(5); }

    }
}