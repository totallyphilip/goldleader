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

        // Star fields
        List<Starfield> starfields = new List<Starfield>();
        starfields.Add(new Starfield(.1, .75)); // slow
        starfields.Add(new Starfield(1, .2)); // fast

        // Make baddies
        Armada badguys = new Armada(1);

        BadGuyField foofighters = new BadGuyField();

        // The player
        Player player = new Player();

        // Power ups
        SpriteField powerups = new SpriteField();

        // keyboard buffer
        List<ConsoleKeyInfo> keybuffer = new List<ConsoleKeyInfo>();

        // Main loop
        int FPS = 2;
        bool UserQuit = false;
        bool debug = true;




        do
        {



            foreach (Starfield starfield in starfields) { starfield.Animate(); }

            badguys.Spawn();
            badguys.Animate();
            player.Animate();
            player.AnimateMissiles();
            player.CheckBadGuyHits(foofighters);
            powerups.Animate();
            foofighters.Animate();
            foofighters.DoStuff();

            // check for hits
            /*            foreach (AsciiEngine.Sprite missile in player.Missiles.Items)
                       {
                           foreach (Ship badguy in badguys.Ships.FindAll(x => x.Alive))
                           {
                               if (badguy.Hit(missile.XY))
                               {
                                   player.Missiles.RemoveSprite(missile);
                               }

                           }


                       } */


            if (debug)
            {
                int x = Screen.LeftEdge;
                int y = Screen.TopEdge;
                Screen.TryWrite(x, y, "[fps: " + FPS + " ships: " + badguys.Ships.Count + " powerups: " + powerups.Items.Count + ']');

                foreach (BadGuy bg in foofighters.Items)
                {
                    y++;
                    Screen.TryWrite(x, y, "[" + new string(bg.Ascii) + " missiles: " + bg.Missiles.Items.Count + ']');


                }


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
                    case ConsoleKey.P:
                        powerups.Items.Add(new PowerUp(PowerUp.ePowerType.ExtraMissile, new Screen.Coordinate(player.XY.X, -1), new Screen.Trajectory(0, 1, Screen.Height)));
                        break;
                }

            }

        } while (!UserQuit);

    }
}