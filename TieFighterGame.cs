using System;
using System.Collections.Generic;
using System.Threading;

public class TieFighterGame
{

    public void TryPlay()
    {

        int oldwidth = Console.WindowWidth;
        int oldheight = Console.WindowHeight;

        if (Textify.SetWindowSizeSafely(80, 25))
        {
            Console.CursorVisible = false;
            this.Play();
            Console.CursorVisible = true;
        }
        else
        {
            Textify.WaitPrompt("something went horribly wrong");
        }

        Textify.SetWindowSizeSafely(oldwidth, oldheight, false);

    }

    public void Play()
    {

        Console.Clear();

        // Make stars
        List<Starfield> backgroundstars = new List<Starfield>();
        backgroundstars.Add(new Starfield(10, Textify.Height / 2, '.')); // slow background
        backgroundstars.Add(new Starfield(0, Textify.Height / 5, '.')); // fast foreground

        // Make baddies
        Armada badguys = new Armada(1);

        // Main loop
        int FPS = 10;
        bool UserQuit = false;
        bool debug = false;

        do
        {

            foreach (Starfield starfield in backgroundstars)
            {
                starfield.Execute();
            }

            badguys.Spawn();
            badguys.Animate();


            if (debug)
            {
                Console.SetCursorPosition(Textify.LeftEdge + 2, Textify.BottomEdge - 2); Console.Write('-'); // scroll bug watcher
                Console.SetCursorPosition(Textify.LeftEdge + 2, Textify.BottomEdge - 1); Console.Write(FPS + " "); // fps display
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
                }
                while (Console.KeyAvailable) { Console.ReadKey(true); } // eat keys

            }

        } while (!UserQuit);

    }
}