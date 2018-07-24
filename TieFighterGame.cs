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

    void StartupScreen()
    {

        Ship fighter = new Ship(Ship.eShipType.Fighter);
        Ship bomber = new Ship(Ship.eShipType.Bomber);
        Ship interceptor = new Ship(Ship.eShipType.Interceptor);
        Ship vader = new Ship(Ship.eShipType.Vader);
        Ship squadron = new Ship(Ship.eShipType.Squadron);

        Console.Clear();
        Console.WriteLine(fighter.Ascii + " - TIE Fighter");
        Console.WriteLine(bomber.Ascii + " - TIE Bomber");
        Console.WriteLine(interceptor.Ascii + " - TIE Interceptor");
        Console.WriteLine(vader.Ascii + " - Darth Vader");
        Console.WriteLine(squadron.Ascii + " - Death Star Trench Squadron");
        Console.Write("press a key");
        Console.ReadKey();

    }

    public void Play()
    {

        StartupScreen();

        Console.Clear();

        // Star fields
        List<Starfield> starfields = new List<Starfield>();
        starfields.Add(new Starfield(.1, .75)); // slow
        starfields.Add(new Starfield(1, .2)); // fast

        // Make baddies
        Armada badguys = new Armada(1);

        BadGuy fooman = null;

        // The player
        Player player = new Player();
        int _MaxMissiles = 2;

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
            powerups.Animate();
            if (fooman != null) { fooman.Animate(); }

            // check for hits
            foreach (AsciiEngine.Sprite missile in player.Missiles.Items)
            {
                foreach (Ship badguy in badguys.Ships.FindAll(x => x.Alive))
                {
                    if (badguy.Hit(missile.XY))
                    {
                        player.Missiles.RemoveSprite(missile);
                    }

                }


            }


            if (debug)
            {
                int x = Screen.LeftEdge;
                int y = Screen.BottomEdge;
                Screen.TryWrite(0, 0, "[fps: " + FPS + " ships: " + badguys.Ships.Count + " powerups: " + powerups.Items.Count + ']');

                if (fooman != null)
                {
                    Screen.TryWrite(0, 1, "[run: " + fooman.Trajectory.Run + " rise: " + fooman.Trajectory.Rise + ']');

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
                        player.Direction = -1;
                        break;
                    case ConsoleKey.RightArrow:
                        player.Direction = 1;
                        break;
                    case ConsoleKey.Spacebar:
                        if (player.Missiles.Items.Count < _MaxMissiles)
                        {
                            player.AddMissile();
                        }
                        break;
                    case ConsoleKey.Escape:
                        UserQuit = true;
                        break;
                    case ConsoleKey.D:
                        debug = !debug;
                        break;
                    case ConsoleKey.P:
                        powerups.Items.Add(new PowerUp(PowerUp.ePowerType.ExtraMissile, new Screen.Coordinate(player.xy.X, -1), new Screen.Trajectory(0, 1, Screen.Height)));
                        fooman = new BadGuy(BadGuy.eBadGuyType.TieFighter);
                        break;
                }

            }

        } while (!UserQuit);

    }
}