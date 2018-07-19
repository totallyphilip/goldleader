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

        if (Screen.TrySetSize(80, 25))
        {
            Console.CursorVisible = false;
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

        // Main loop
        int FPS = 10;
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
                    if (badguy.Hit(missile.X, missile.Y))
                    {
                        badguys.HurtShip(badguy, 1);
                        p.Missiles.RemoveSprite(missile);
                    }

                }


            }


            if (debug)
            {
                Console.SetCursorPosition(Screen.LeftEdge + 2, Screen.BottomEdge - 2); Console.Write('-'); // scroll bug watcher
                Console.SetCursorPosition(Screen.LeftEdge + 2, Screen.BottomEdge - 1); Console.Write(FPS + " "); // fps display
            }

            Thread.Sleep(1000 / FPS);

            if (Console.KeyAvailable)
            {

                ConsoleKeyInfo k = Console.ReadKey(true);
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
                while (Console.KeyAvailable) { Console.ReadKey(true); } // eat keys

            }

        } while (!UserQuit);

    }
}