using System;
using System.Collections.Generic;
using System.Threading;
public class TieFight
{

    public void TryPlay()
    {

        int oldwidth = Console.WindowWidth;
        int oldheight = Console.WindowHeight;

        if (Textify.SetWindowSizeSafely(50, 35))
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
        List<StarField> starlayer = new List<StarField>();
        starlayer.Add(new StarField(10, Textify.Height / 2, '.')); // slow background
        starlayer.Add(new StarField(0, Textify.Height / 5, '.')); // fast foreground

        // Make baddies
        EnemyFleet fleet = new EnemyFleet(1);

        // Main loop
        int FPS = 10;
        bool UserQuit = false;
        bool debug = false;

        do
        {

            foreach (StarField stars in starlayer)
            {
                stars.Execute();
            }
            fleet.Spawn();


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