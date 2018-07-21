using AsciiEngine;
using System;
using System.Collections.Generic;
using System.Threading;

public class TieFighterGame
{

    public void TryPlay()
    {

        int oldwidth = Console.WindowWidth;
        int oldheight = Console.WindowHeight;

        if (true || Screen.TrySetSize(40, 30))
        {
            Console.CursorVisible = false;
            Screen.Countdown(5);
            Easy.Keys.EatKeys();
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

        // Make stars
        List<Starfield> starfields = new List<Starfield>();
        starfields.Add(new Starfield(.1, .75)); // slow
        starfields.Add(new Starfield(1, .2)); // fast

        // Make baddies
        Armada badguys = new Armada(1);

        // The player
        Player p = new Player();
        int _MaxMissiles = 2;

        // keyboard buffer
        List<ConsoleKeyInfo> keybuffer = new List<ConsoleKeyInfo>();

        // Main loop
        int FPS = 2;
        bool UserQuit = false;
        bool debug = false;

        do
        {

            foreach (Starfield starfield in starfields) { starfield.Animate(); }

            badguys.Spawn();
            badguys.Animate();
            p.Animate();

            // check for hits
            foreach (AsciiEngine.Sprite missile in p.Missiles.Sprites)
            {
                foreach (Ship badguy in badguys.Ships)
                {
                    if (badguy.Hit(missile.XY.X, missile.XY.Y))
                    {
                        p.Missiles.RemoveSprite(missile);
                    }

                }


            }


            if (debug)
            {
                int x = Screen.LeftEdge;
                int y = Screen.BottomEdge;
                Screen.TryWrite(0, 0, "[fps: " + FPS + " ships: " + badguys.Ships.Count + ']');
            }

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
                 ) { keybuffer.Clear(); }
                keybuffer.Add(k);
            }

            if (keybuffer.Count > 0)
            {

                ConsoleKeyInfo k = keybuffer[0];
                keybuffer.Remove(k);
                switch (k.Key)
                {
                    case ConsoleKey.UpArrow:
                        FPS++;
                        break;
                    case ConsoleKey.DownArrow:
                        if (FPS - 1 > 0) { FPS--; }
                        break;
                    case ConsoleKey.LeftArrow:
                        p.Direction = -1;
                        break;
                    case ConsoleKey.RightArrow:
                        p.Direction = 1;
                        break;
                    case ConsoleKey.Spacebar:
                        if (p.Missiles.Sprites.Count < _MaxMissiles)
                        {
                            p.AddMissile();
                        }
                        break;
                    case ConsoleKey.Escape:
                        UserQuit = true;
                        break;
                    case ConsoleKey.D:
                        debug = !debug;
                        break;
                }

            }

        } while (!UserQuit);

    }
}