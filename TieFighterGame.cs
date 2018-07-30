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

        // int oldwidth = Console.WindowWidth;
        // int oldheight = Console.WindowHeight;

        if (LinuxDevMode || Screen.TrySetSize(45, 35))
        {
            Console.CursorVisible = false;
            Easy.Keyboard.EatKeys();
            this.MainLoop();
            Console.CursorVisible = true;
        }
        else
        {
            Console.Write("something went horribly wrong");
            Console.ReadKey();
        }

        // Screen.TrySetSize(oldwidth, oldheight, false);

    }

    void MainLoop()
    {
        do
        {
            if (!GetTheFkOut) { Attract(); }
            if (!GetTheFkOut) { PlayTheGame(); }
        } while (!GetTheFkOut);
    }

    public void Attract()
    {
        Console.Clear();
        Swarm badguys = new Swarm();
        Starfield stars = new Starfield(.2, .5);

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
        Messages.Add("Press Esc to Quit");
        Messages.Add("Press Enter to Begin");


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
            stars.Animate();
            Scroller.Animate();
            Easy.Clock.FpsThrottle(8);
        } while (!Console.KeyAvailable);

        GetTheFkOut = Console.ReadKey(true).Key == ConsoleKey.Escape;

    }

    void PlayTheGame()
    {

        Console.Clear();

        // starfield
        List<Starfield> starfields = new List<Starfield>();
        starfields.Add(new Starfield(.1, .75)); // slow
        starfields.Add(new Starfield(1, .2)); // fast

        // the user
        Player player = new Player();
        int InitialShields = 5;
        player.HP = InitialShields;
        Score = 0;

        // misc
        int Hyperdrive = 0;
        int Round = 0;
        bool Paused = false;

        // define the waves of bad guys

        List<EnemyWave> waves = new List<EnemyWave>();

        EnemyWave newwave;

        newwave = new EnemyWave(100, "Great, Kid! Don't get cocky.", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 1));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(100, "Like bull's-eying womp rats in a T-16.", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 3));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(100, "", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 4));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Bomber, 2));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(2, "", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Leader, 4));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(1, "", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Interceptor, 3));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(100, "That armor's too strong for blasters!", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Squadron, 2));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Bomber, 3));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(6, "Never tell me the odds!", true);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 3));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Interceptor, 3));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Leader, 3));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(6, "", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 3));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Interceptor, 3));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Leader, 3));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Bomber, 3));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(12, "Stay on target!", true);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 60));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(6, "", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Squadron, 3));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Bomber, 6));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 4));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(8, "", true);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Squadron, 1));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Bomber, 2));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 4));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Interceptor, 4));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Leader, 4));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(6, "It's a trap!", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 6));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Interceptor, 6));
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Leader, 6));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(1, "Now, young Skywalker, you will die.", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(BadGuy.eBadGuyType.Fighter, 4));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(false);
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        foreach (EnemyWave wave in waves)
        {

            Scroller Scroller = new Scroller(2, Screen.Height / 3, .25);

            bool FarmBoyTaunted = false;

            // display instructions
            if (Round == 0)
            {
                Scroller.NewLine("Space = Fire");
                Scroller.NewLine("Left/Right = Move");
                Scroller.NewLine("Up/Down = Faster/Slower");
                Scroller.NewLine("Tab = Hyperdrive");
                Scroller.NewLine("Enter = Pause");
                Scroller.NewLine("Esc = Quit");
                Scroller.NewLine("");
                Scroller.NewLine("");
                Scroller.NewLine("");
                Scroller.NewLine("");
                Scroller.NewLine("");
            }

            // display round number
            Round++;
            if (wave.Infinite)
            {
                Scroller.NewLine("May the Force be with you!");
                player.HP = InitialShields;
            }
            else { Scroller.NewLine("Round " + Round); }

            // display shields message
            Scroller.NewLine("Deflector shield " + (Convert.ToDouble(player.HP - 1) / (InitialShields - 1)) * 100 + "% charged.");

            if (wave.WeaponsUpgrade)
            {
                Scroller.NewLine("");
                player.MaxMissiles++;
                Scroller.NewLine("Blaster upgraded!");
            }

            // keyboard buffer
            List<ConsoleKeyInfo> keybuffer = new List<ConsoleKeyInfo>();

            do
            {

                // animate
                foreach (Starfield starfield in starfields) { starfield.Animate(); }

                if (Paused)
                {
                    player.Redraw();
                    if (player.Debris != null) { player.Debris.Redraw(); }
                    player.Missiles.Redraw();
                    wave.Redraw();
                    foreach (BadGuy bg in wave.Items)
                    {
                        bg.Sparks.Redraw();
                        bg.Debris.Redraw();
                        bg.Messages.Redraw();
                        bg.Missiles.Redraw();
                    }
                    if (Scroller.Empty) { Scroller.NewLine("Press Enter to resume."); }
                }
                else
                {
                    if (player.Alive) { player.Animate(); }
                    if (player.Active) { player.DoActivities(); }

                    wave.CheckCollisions(player.Missiles);
                    wave.Animate();
                    wave.CheckCollisions(player.Missiles);

                    foreach (BadGuy bg in wave.Items)
                    {
                        bg.Missiles.CheckCollision(player);
                    }
                }


                // debugging
                if (ShowDebugInfo)
                {
                    AsciiEngine.Screen.TryWrite(new Point(0, 0), "FPS:" + FramesPerSecond + " ");
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
                            if (FramesPerSecond > 60) { FramesPerSecond = 60; }
                            break;
                        case ConsoleKey.DownArrow:
                            FramesPerSecond--;
                            if (FramesPerSecond < 2) { FramesPerSecond = 2; }
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
                        case ConsoleKey.Enter:
                            Paused = !Paused;
                            break;
                        case ConsoleKey.T:
                            Score = 0;
                            player.HP++;
                            Scroller.NewLine("Cheater! No score for you!");
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
                    Scroller.NewLine("");
                    Scroller.NewLine("");
                    Scroller.NewLine("They came from behind!");
                }
                if (wave.WaveDefeated() && !wave.Congratulated)
                {
                    wave.Congratulated = true;
                    if (wave.WinMessage == "") { Scroller.NewLine("Wave cleared."); }
                    else { Scroller.NewLine(wave.WinMessage); }
                }

                Console.Title = "Score: " + Score;

                // throttle the cpu
                if (Hyperdrive > 0)
                {
                    Hyperdrive--;
                    if (Hyperdrive == 0 && !FarmBoyTaunted)
                    {
                        Scroller.NewLine("Traveling through hyperspace");
                        Scroller.NewLine("ain't like dusting crops, farm boy.");
                        FarmBoyTaunted = true;
                    }

                }
                else { if (!GetTheFkOut) { Easy.Clock.FpsThrottle(FramesPerSecond); } }


            } while (!GetTheFkOut && (!Scroller.Empty || (player.Active && !wave.WaveDefeated())));

            if (!player.Alive) { break; }

        }


    }
}