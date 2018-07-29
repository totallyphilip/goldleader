using AsciiEngine;
using AsciiEngine.Fx;
using AsciiEngine.Grid;
using AsciiEngine.Sprites;
using System;
using System.Collections.Generic;

public class TieFighterGame
{

    public static bool GetTheFkOut = false;
    public static bool ShowDebugInfo = false;
    int FramesPerSecond = 8;

    public static int Score;
    public void TryPlay()
    {

        bool LinuxDevMode = true;

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


        Scroller Scroller = new Scroller(2, Screen.Height / 2, .5);
        do
        {
            if (Scroller.Empty)
            {
                foreach (string s in Messages)
                {
                    Scroller.NewLine(s);
                }
            }
            badguys.Animate();
            Scroller.Animate();
            Easy.Clock.FpsThrottle(8);
        } while (!Console.KeyAvailable && !GetTheFkOut);

    }

    void Looper()
    {
        do
        {
            Demo();
            PlayTheGame();
        } while (!GetTheFkOut);
    }

    void PlayTheGame()
    {

        // starfield
        List<Starfield> starfields = new List<Starfield>();
        starfields.Add(new Starfield(.1, .75)); // slow
        starfields.Add(new Starfield(1, .2)); // fast

        // The player
        Player player = new Player();
        player.HP = 3;

        // misc
        int Hyperdrive = 0;


        List<EnemyWave> waves = new List<EnemyWave>();

        EnemyWave newwave;

        newwave = new EnemyWave(3, "Ready!", "Like bull's-eying womp rats in a T-16.", "Maybe you should stay on the farm.");
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 3));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(3, "Ready!", "Nice.", "You should have gone to Tosche Station.");
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Bomber, 3));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(3, "Ready?", "You lived.", "I think somebody up there doesn't like you.");
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Interceptor, 3));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 3));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);


        foreach (EnemyWave wave in waves)
        {

            Console.Clear();
            Scroller Scroller = new Scroller(2, Screen.Height / 2, .25);

            Scroller.NewLine(wave.ReadyMessage);

            // keyboard buffer
            List<ConsoleKeyInfo> keybuffer = new List<ConsoleKeyInfo>();

            do
            {



                foreach (Starfield starfield in starfields) { starfield.Animate(); }

                if (player.Alive) { player.Animate(); }
                if (player.Active) { player.DoActivities(); }

                wave.CheckCollisions(player.Missiles);
                wave.Animate();
                wave.CheckCollisions(player.Missiles);

                foreach (BadGuy bg in wave.Items)
                {
                    bg.Missiles.CheckCollision(player);
                }

                // throttle the cpu
                if (Hyperdrive > 0) { Hyperdrive--; }
                else { Easy.Clock.FpsThrottle(FramesPerSecond); }


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
                            Hyperdrive = 20;
                            break;
                        case ConsoleKey.UpArrow:
                            FramesPerSecond++;
                            break;
                        case ConsoleKey.DownArrow:
                            FramesPerSecond--;
                            if (FramesPerSecond < 1) { FramesPerSecond = 1; }
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
                            GetTheFkOut = true;
                            break;
                        case ConsoleKey.D:
                            ShowDebugInfo = !ShowDebugInfo;
                            break;
                    }

                }

                // display messages
                Scroller.Animate();
                if (Scroller.Empty) { wave.StartAttackRun(); }
                if (!player.Alive && !wave.Humiliated)
                {
                    wave.Humiliated = true;
                    Scroller.NewLine(wave.LoseMessage);
                }
                if (wave.WaveDefeated() && !wave.Congratulated)
                {
                    wave.Congratulated = true;
                    Scroller.NewLine(wave.WinMessage);
                }


            } while (!GetTheFkOut && (!Scroller.Empty || (player.Active && !wave.WaveDefeated())));

            if (!player.Alive) { break; }

        }


    }
}