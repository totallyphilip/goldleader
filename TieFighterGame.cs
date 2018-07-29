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
        Messages.Add("");
        Messages.Add("- Rounds -");
        Messages.Add("You can survive two hits per round.");
        Messages.Add("");
        Messages.Add("PRESS ANY KEY TO BEGIN");


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
        int Round = 0;

        List<EnemyWave> waves = new List<EnemyWave>();

        EnemyWave newwave;

        newwave = new EnemyWave(3, "Great, Kid! Don't get cocky.", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 1));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(3, "Like bull's-eying womp rats in a T-16.", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 3));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(3, "You are the bomb!", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Bomber, 3));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(4, "Keep going!", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 3));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Leader, 1));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(6, "Keep going!", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Interceptor, 2));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 4));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(4, "The Force is with you.", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Interceptor, 2));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 4));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Leader, 2));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(5, "The Force is with you.", true);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 8));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Squadron, 2));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Leader, 1));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(6, "Keep going!", true);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 8));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Squadron, 2));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Leader, 2));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Bomber, 6));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(8, "Rebel scum. Time to die.", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 4));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Bomber, 4));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Interceptor, 4));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Leader, 4));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Squadron, 4));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(true);
        newwave.CreateIncomingFleet();
        waves.Add(newwave);



        foreach (EnemyWave wave in waves)
        {

            Console.Clear();
            Scroller Scroller = new Scroller(2, Screen.Height / 3, .25);


            Round++;
            Scroller.NewLine("Round " + Round);
            Scroller.NewLine("");
            if (wave.WeaponsUpgrade)
            {
                player.MaxMissiles++;
                Scroller.NewLine("Blaster upgraded!");
                Scroller.NewLine("");
            }
            player.HP = 3;

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
                    Scroller.NewLine("G A M E   O V E R");
                }
                if (wave.WaveDefeated() && !wave.Congratulated)
                {
                    wave.Congratulated = true;
                    Scroller.NewLine(wave.WinMessage);
                }

                // throttle the cpu
                if (Hyperdrive > 0) { Hyperdrive--; }
                else { Easy.Clock.FpsThrottle(FramesPerSecond); }


            } while (!GetTheFkOut && (!Scroller.Empty || (player.Active && !wave.WaveDefeated())));

            if (!player.Alive) { break; }

        }


    }
}