using System;
using System.Collections.Generic;
using System.Threading;

public class TieFighterGame
{

    public void TryPlay()
    {

        int oldwidth = Console.WindowWidth;
        int oldheight = Console.WindowHeight;

        if (AsciiEngine.TrySetWindowSize(80, 25))
        {
            Console.CursorVisible = false;
            this.Play();
            Console.CursorVisible = true;
        }
        else
        {
            AsciiEngine.CharPrompt("something went horribly wrong");
        }

        AsciiEngine.TrySetWindowSize(oldwidth, oldheight, false);

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

        // Main loop
        int FPS = 10;
        bool UserQuit = false;
        bool debug = false;

        do
        {

            foreach (Starfield starfield in starfields) { starfield.Animate(); }

            badguys.Spawn();
            badguys.Animate();


            if (debug)
            {
                Console.SetCursorPosition(AsciiEngine.LeftEdge + 2, AsciiEngine.BottomEdge - 2); Console.Write('-'); // scroll bug watcher
                Console.SetCursorPosition(AsciiEngine.LeftEdge + 2, AsciiEngine.BottomEdge - 1); Console.Write(FPS + " "); // fps display
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
                    case ConsoleKey.Escape:
                        UserQuit = true;
                        break;
                    case ConsoleKey.D:
                        debug = !debug;
                        break;
                    case ConsoleKey.X:
                        badguys.test();
                        break;
                }
                while (Console.KeyAvailable) { Console.ReadKey(true); } // eat keys

            }

        } while (!UserQuit);

    }
}