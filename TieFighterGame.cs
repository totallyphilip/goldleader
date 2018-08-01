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
    int FramesPerSecond = 9;

    public static int Score;
    public void TryPlay()
    {

        bool LinuxDevMode = false;

        // int oldwidth = Console.WindowWidth;
        // int oldheight = Console.WindowHeight;

        if (LinuxDevMode || Screen.TrySetSize(45, 35))
        {
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

        foreach (Enemy.eEnemyType shiptype in (Enemy.eEnemyType[])System.Enum.GetValues(typeof(Enemy.eEnemyType)))
        {
            Enemy bg = new Enemy(shiptype);
            badguys.Items.Add(bg);
            Messages.Add(new string(bg.Ascii) + " " + Enum.GetName(typeof(Enemy.eEnemyType), shiptype) + " (" + bg.HP + " HP)");
        }
        Messages.Add("");

        Messages.Add("- Scoring -");
        Messages.Add("Hit = 1 X Altitude Bonus");
        Messages.Add("Kill = 2 X Altitude Bonus");
        Messages.Add("");
        Messages.Add("Press Esc to Quit");
        Messages.Add("Press Space to Begin");
        Messages.Add("");
        Messages.Add("");
        Messages.Add("");
        Messages.Add("");
        Messages.Add("");


        Scroller Scroller = new Scroller(2, Screen.Height / 2, .5);
        do
        {
            Console.CursorVisible = false;

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

    enum eHyperdriveMode { Unused, Engaged, Disengaged };

    void PlayTheGame()
    {

        Console.Clear();

        // starfield
        Galaxy stars = new Galaxy();

        // the user
        Player player = new Player();
        int ShieldMax = 5;
        player.HP = 0;
        Score = 0;

        // hyperdrive
        eHyperdriveMode HyperdriveMode = eHyperdriveMode.Unused;
        int hyperbonus = 0;

        // misc
        int Round = 0;
        bool Paused = false;
        bool HeroBonusGiven = false;

        #region  " Waves "

        // define the waves of bad guys

        List<EnemyWave> waves = new List<EnemyWave>();

        EnemyWave newwave;

        newwave = new EnemyWave(100, "Great, Kid! Don't get cocky.", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Fighter, 1));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(100, "Like bull's-eying womp rats in a T-16.", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Fighter, 3));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(100, "", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Fighter, 4));
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Bomber, 2));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(2, "", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Leader, 4));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(1, "", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Fighter, 2));
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Interceptor, 2));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(8, "", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.BomberII, 2));
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Fighter, 4));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(100, "That armor's too strong for blasters!", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Squadron, 2));
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Bomber, 3));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(6, "Never tell me the odds!", true);
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Fighter, 3));
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Interceptor, 3));
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Leader, 3));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(6, "", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Fighter, 3));
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Interceptor, 3));
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Leader, 3));
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.BomberII, 1));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(12, "Stay on target!", true);
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Fighter, 60));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(6, "", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Squadron, 3));
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Bomber, 6));
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Fighter, 4));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(8, "", true);
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.BomberII, 1));
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Bomber, 2));
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Fighter, 4));
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Interceptor, 4));
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Leader, 4));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(6, "It's a trap!", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Fighter, 6));
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Interceptor, 6));
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Leader, 6));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(1, "Now, young Skywalker, you will die.", false);
        newwave.Fleet.Add(new EnemyWave.Squadron(Enemy.eEnemyType.Fighter, 4));
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        newwave = new EnemyWave(false);
        newwave.CreateIncomingFleet();
        waves.Add(newwave);

        #endregion

        foreach (EnemyWave wave in waves)
        {

            Scroller Scroller = new Scroller(2, Screen.Height / 3, .25);
            bool ClosingWordsStated = false;
            player.HP++;

            // display instructions
            if (Round == 0)
            {
                Scroller.NewLine("Space = Fire");
                Scroller.NewLine("Left/Right = Move");
                //Scroller.NewLine("PgUp/PgDn = Faster/Slower");
                Scroller.NewLine("Up = Hyperdrive");
                //Scroller.NewLine("Enter = Pause");
                Scroller.NewLine("Esc = Quit");
                Scroller.NewLine(4);
            }

            // display round number
            Round++;
            if (wave.Infinite)
            {
                Scroller.NewLine("May the Force be with you!");
                player.HP = ShieldMax;
            }
            else { Scroller.NewLine("Round " + Round); }

            // display shields message
            Scroller.NewLine("Deflector shield " + (Convert.ToDouble(player.HP - 1) / (ShieldMax - 1)) * 100 + "% charged.");

            // reset hyperdrive
            if (HyperdriveMode == eHyperdriveMode.Disengaged) { Scroller.NewLine("Navicomputer coordinates recalculated."); }
            HyperdriveMode = eHyperdriveMode.Unused;
            if (hyperbonus < 1) { hyperbonus = Round * 10; } else { hyperbonus += Round * 10; }

            // upgrade weapons
            if (wave.WeaponsUpgrade)
            {
                Scroller.NewLine();
                player.MaxMissiles++;
                Scroller.NewLine("Blaster upgraded!");
            }

            // keyboard buffer
            List<ConsoleKeyInfo> keybuffer = new List<ConsoleKeyInfo>();

            do
            {
                Console.CursorVisible = false; // windows turns the cursor back on when restoring from minimized window

                // animate
                stars.Animate();

                if (Paused)
                {
                    player.Refresh();
                    if (player.Debris != null) { player.Debris.Refresh(); }
                    player.Missiles.Refresh();
                    wave.Refresh();
                    if (Scroller.Empty) { Scroller.NewLine("Press Enter to resume."); }
                }
                else
                {

                    if (HyperdriveMode == eHyperdriveMode.Engaged)
                    {
                        stars.Animate();
                        if (Easy.Clock.Elapsed(3))
                        {
                            HyperdriveMode = eHyperdriveMode.Disengaged;
                            wave.ExitHyperspace();
                            stars.ExitHyperspace();
                        }
                    }
                    else if (HyperdriveMode == eHyperdriveMode.Unused)
                    {
                        wave.CheckCollisions(player.Missiles);
                        wave.Animate();
                        wave.CheckCollisions(player.Missiles);

                        foreach (Enemy bg in wave.Items)
                        {
                            bg.Missiles.CheckCollision(player);
                        }

                        string ShieldMarkers = "";
                        if (player.HP > 0) { ShieldMarkers = new String('X', player.HP - 1); }
                        Screen.TryWrite(new Point(1, player.XY.iY + 1), ShieldMarkers + ' ');
                        string ShotMarkers = " " + new String('|', player.MaxMissiles - player.Missiles.Items.Count);
                        Screen.TryWrite(new Point(Screen.Width - ShotMarkers.Length - 1, player.XY.iY + 1), ShotMarkers);

                    }

                    if (player.Alive) { player.Animate(); }
                    if (player.Active) { player.Activate(); }

                }


                // debugging
                if (ShowDebugInfo)
                {
                    AsciiEngine.Screen.TryWrite(new Point(0, 0), "FPS:" + FramesPerSecond + " HP:" + player.HP);
                }


                // wipe the keyboard buffer if a priority key is pressed
                while (Console.KeyAvailable)
                {
                    ConsoleKeyInfo k = Console.ReadKey(true);
                    if (k.Key == ConsoleKey.Tab) // prioritize emergency hyperspace
                    {
                        keybuffer.Clear();
                        keybuffer.Add(k);
                    }
                    else if (
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
                        case ConsoleKey.UpArrow:
                            if (HyperdriveMode == eHyperdriveMode.Unused && !wave.Completed() && player.Alive && !wave.Infinite)
                            {
                                stars.EnterHyperspace();
                                player.Trajectory.Run = 0;
                                player.Missiles.TerminateAll();
                                HyperdriveMode = eHyperdriveMode.Engaged;
                                Easy.Clock.StartTimer();
                            }
                            break;
                        case ConsoleKey.PageUp:
                            FramesPerSecond++;
                            if (FramesPerSecond > 60) { FramesPerSecond = 60; }
                            break;
                        case ConsoleKey.PageDown:
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
                            if (HyperdriveMode != eHyperdriveMode.Engaged) { player.Fire(); }
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
                            player.OnHit();
                            //Score = 0;
                            //player.HP++;
                            //Scroller.NewLine("Deflector shield " + (Convert.ToDouble(player.HP - 1) / (InitialShields - 1)) * 100 + "% charged.");
                            //Scroller.NewLine("Score reset to zero.");
                            break;
                    }

                }

                // display messages
                Scroller.Animate();
                if (Scroller.Empty) { wave.StartAttackRun(); }

                if (!player.Alive && !ClosingWordsStated)
                {
                    ClosingWordsStated = true;
                    Scroller.NewLine("G A M E   O V E R");
                    Scroller.NewLine(3);
                    Scroller.NewLine("They came from behind!");
                }

                if (player.Alive && wave.Completed() && !ClosingWordsStated)
                {
                    ClosingWordsStated = true;

                    if (HyperdriveMode == eHyperdriveMode.Disengaged)
                    {
                        Scroller.NewLine("Traveling through hyperspace");
                        Scroller.NewLine("ain't like dusting crops, boy.");
                        hyperbonus = 0;
                    }
                    else if (wave.WinMessage == "") { Scroller.NewLine("Wave cleared."); }
                    else { Scroller.NewLine(wave.WinMessage); }


                    Scroller.NewLine("+" + hyperbonus + " navicomputer bonus.");
                    Score += hyperbonus;
                }

                if (!player.Alive && wave.Completed() && !HeroBonusGiven)
                {
                    int deadherobonus = (Round * 100) / 2;
                    Scroller.NewLine(3);
                    Scroller.NewLine("That");
                    Scroller.NewLine("was");
                    Scroller.NewLine("awesome.");
                    Scroller.NewLine(3);
                    Scroller.NewLine("+" + deadherobonus + " Dead hero bonus.");
                    Score += deadherobonus;
                    HeroBonusGiven = true;
                }

                Console.Title = "Score: " + Score;

                // throttle the cpu
                if (!GetTheFkOut)
                {
                    if (HyperdriveMode == eHyperdriveMode.Engaged) { Easy.Clock.FpsThrottle(100); }
                    else { Easy.Clock.FpsThrottle(FramesPerSecond); }
                }

            } while (!GetTheFkOut && (!Scroller.Empty || (player.Active && !wave.Completed())));

            if (!player.Alive) { break; }

        }


    }
}