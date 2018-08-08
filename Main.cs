using AsciiEngine;
using AsciiEngine.Fx;
using AsciiEngine.Grid;
using AsciiEngine.Sprites;
using System;
using System.Collections.Generic;

public class AsciiWars
{

    public static bool GetTheFkOut = false;
    //    public static bool ShowDebugInfo = false;
    int FramesPerSecond = 9;
    bool ContinuousPlay = true;

    public AsciiWars() { ContinuousPlay = true; }
    public AsciiWars(bool cp) { ContinuousPlay = cp; }

    public int TryPlay(int HighScore)
    {

        GetTheFkOut = false;
        int Score = 0;

        bool LinuxDevMode = false;

        int oldwidth = Console.WindowWidth;
        int oldheight = Console.WindowHeight;

        if (LinuxDevMode || Screen.TrySetSize(50, 40))
        {
            Easy.Keyboard.EatKeys();
            Score = this.MainLoop(HighScore);
            Console.CursorVisible = true;
        }
        else
        {
            Console.Write("something went horribly wrong");
            Console.ReadKey();
        }

        Screen.TrySetSize(oldwidth, oldheight, false);

        return Score;
    }

    int MainLoop(int HighScore)
    {
        int Score = 0;
        do
        {
            if (!GetTheFkOut) { Attract(); }
            if (!GetTheFkOut) { Score = PlayTheGame(HighScore); }
        } while (!GetTheFkOut && ContinuousPlay);
        return Score;
    }

    public void Attract()
    {
        Console.Clear();
        Swarm badguys = new Swarm();
        Starfield stars = new Starfield(.2, .5);

        List<string> Messages = new List<string>();

        Messages.Add("A S C I I   W A R S");
        Messages.Add("");
        foreach (Enemy.eEnemyType shiptype in (Enemy.eEnemyType[])System.Enum.GetValues(typeof(Enemy.eEnemyType)))
        {
            Enemy bg = new Enemy(shiptype);
            badguys.Items.Add(bg);
            Messages.Add(new string(bg.Ascii) + " " + Enum.GetName(typeof(Enemy.eEnemyType), shiptype) + " (" + bg.HP + " HP)");
        }
        Messages.Add("");
        Messages.Add("Press Esc to Quit");
        Messages.Add("Press Space to Begin");
        Messages.Add("");
        Messages.Add("");
        Messages.Add("");


        Scroller Scroller = new Scroller(2, Screen.Height / 2, .25);
        do
        {
            Console.CursorVisible = false;

            if (badguys.Empty)
            {
                foreach (Enemy.eEnemyType shiptype in (Enemy.eEnemyType[])System.Enum.GetValues(typeof(Enemy.eEnemyType)))
                {
                    badguys.Items.Add(new Enemy(shiptype));
                }
            }

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

            if (Easy.Abacus.RandomTrue && Easy.Abacus.RandomTrue && Easy.Abacus.RandomTrue && Easy.Abacus.RandomTrue)
            {
                int r = Easy.Abacus.Random.Next(0, badguys.Count);
                if (badguys.Items[r].Alive) { badguys.Items[r].OnHit(); }
            }

        } while (!Console.KeyAvailable);

        GetTheFkOut = Console.ReadKey(true).Key == ConsoleKey.Escape;

    }

    enum eHyperdriveMode { Unused, Engaged, Disengaged };

    int PlayTheGame(int HighScore)
    {

        Console.Clear();

        // starfield
        Galaxy stars = new Galaxy();

        // the user
        Player player = new Player();
        int ShieldMax = 5;
        player.HP = 0;
        int Score = 0;

        // hyperdrive
        eHyperdriveMode HyperdriveMode = eHyperdriveMode.Unused;
        Easy.Abacus.Fibonacci hyperbonus = new Easy.Abacus.Fibonacci();

        // misc
        bool Paused = false;
        bool HeroBonusGiven = false;

        #region  " Waves "

        // define the waves of bad guys

        List<EnemyWave> Waves = new List<EnemyWave>();

        EnemyWave Wave;

        Wave = new EnemyWave(100, "", "Great, Kid! Don't get cocky.", false);
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 1));
        Waves.Add(Wave);

        Wave = new EnemyWave(100, "", "Like bull's-eying womp rats in a T-16.", false);
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 3));
        Waves.Add(Wave);

        Wave = new EnemyWave(6, "", "", false);
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 2, Enemy.eEnemyType.Fighter));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 4));
        Waves.Add(Wave);

        Wave = new EnemyWave(100, "", "", false);
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Bomber, 2));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 2, Enemy.eEnemyType.Vanguard));
        Waves.Add(Wave);

        Wave = new EnemyWave(100, "", "Great shot, Kid! That was one in a million!", false);
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Interdictor, 1, Enemy.eEnemyType.Fighter));
        Waves.Add(Wave);

        Wave = new EnemyWave(20, "I've got a bad feeling about this.", "The force is strong with this one.", true); // too hard, force hyperspace
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 10));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.HeavyBomber, 10));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 20));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.HeavyBomber, 5));
        Waves.Add(Wave);

        Wave = new EnemyWave(100, "", "", false);
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Interdictor, 2, Enemy.eEnemyType.HeavyFighter));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 6));
        Waves.Add(Wave);

        Wave = new EnemyWave(3, "", "", false);
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 2));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Interceptor, 2));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.HeavyBomber, 2));
        Waves.Add(Wave);

        Wave = new EnemyWave(100, "", "", false);
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Leader, 2, Enemy.eEnemyType.HeavyFighter));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Bomber, 3));
        Waves.Add(Wave);

        Wave = new EnemyWave(6, "Fighters incoming!", "", true);
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 3));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Interceptor, 3));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Leader, 3));
        Waves.Add(Wave);

        Wave = new EnemyWave(6, "", "", false);
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 3));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Interceptor, 3));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Leader, 3));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.HeavyBomber, 1));
        Waves.Add(Wave);


        Wave = new EnemyWave(12, "Never tell me the odds!", "", true);
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 4, Enemy.eEnemyType.Fighter));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 12));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 4, Enemy.eEnemyType.Fighter));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 12));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 4, Enemy.eEnemyType.Fighter));
        Waves.Add(Wave);


        Wave = new EnemyWave(12, "", "", false);
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Bomber, 1));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Leader, 1, Enemy.eEnemyType.HeavyFighter));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.HeavyBomber, 1));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Leader, 1, Enemy.eEnemyType.HeavyFighter));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Bomber, 1));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Leader, 1, Enemy.eEnemyType.HeavyFighter));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Bomber, 1));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 2, Enemy.eEnemyType.Fighter));
        Waves.Add(Wave);

        Wave = new EnemyWave(8, "", "", false);
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.HeavyBomber, 1));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Bomber, 2));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 4));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Interceptor, 4));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Leader, 4));
        Waves.Add(Wave);

        Wave = new EnemyWave(6, "", "It's a trap!", false);
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 6));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Interceptor, 6));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Leader, 6));
        Waves.Add(Wave);

        Wave = new EnemyWave(1, "", "They let us go.", false);
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 4));
        Waves.Add(Wave);

        Wave = new EnemyWave(15, "Now, young Skywalker, you will die.", "", false);
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Leader, 5, Enemy.eEnemyType.Fighter));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Vanguard, 10));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 20));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.HeavyFighter, 5));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Interceptor, 5));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Bomber, 1));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.HeavyBomber, 1));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Interdictor, 1));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 10, Enemy.eEnemyType.Fighter));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Leader, 5));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.HeavyFighter, 20));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Bomber, 5));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 5));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.HeavyBomber, 5));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 5));
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Interdictor, 1000)); // not survivable
        Waves.Add(Wave);

        #endregion

        // display instructions
        Scroller Scroller = new Scroller(2, Screen.Height / 3, .25, 1.5);
        Scroller Instructions = new Scroller(2, Screen.Height / 3, .25, 1.5);
        Scroller.NewLine("Enter = Instructions");
        Scroller.NewLine("Esc = Quit");
        Scroller.NewLine();
        if (HighScore > 0)
        {
            Scroller.NewLine("Beat your high score: " + HighScore);
            Scroller.NewLine();
        }

        foreach (EnemyWave wave in Waves)
        {

            bool ClosingWordsStated = false;
            player.HP++;

            if (Waves.IndexOf(wave) == Waves.Count - 1)
            {
                player.HP += 4;
                Scroller.NewLine("Final wave!");
                Scroller.NewLine("Deflector shield boosted.");
                Scroller.NewLine("May the force be with you.");
            }
            else
            {
                Scroller.NewLine("Wave " + (Waves.IndexOf(wave) + 1));
                // display shields message
                Scroller.NewLine("Deflector shield " + (Convert.ToDouble(player.HP - 1) / (ShieldMax - 1)) * 100 + "% charged.");
                // reset hyperdrive
                if (HyperdriveMode == eHyperdriveMode.Disengaged) { Scroller.NewLine("Navicomputer coordinates recalculated."); }

            }
            if (wave.HasWelcomeMessage) { Scroller.NewLine(wave.PopWelcomeMessage()); }

            HyperdriveMode = eHyperdriveMode.Unused;







            // upgrade weapons
            if (wave.WeaponsUpgrade)
            {
                player.UpgradeBlasters();
                Scroller.NewLine();
                Scroller.NewLine("Blasters upgraded!");
            }

            // keyboard buffer
            List<ConsoleKeyInfo> keybuffer = new List<ConsoleKeyInfo>();

            do
            {

                if (Scroller.Empty) { wave.StartAttackRun(); } // wait until wave intro messages are gone

                Console.CursorVisible = false; // windows turns the cursor back on when restoring from minimized window

                // animate
                stars.Animate();

                if (Paused)
                {
                    player.Refresh();
                    if (player.Debris != null) { player.Debris.Refresh(); }
                    player.Missiles.Refresh();
                    wave.Refresh();
                    if (Instructions.Empty)
                    {
                        Scroller.HideAll();
                        Instructions.NewLine("Press Enter to resume game.");
                        Instructions.NewLine();
                        Instructions.NewLine("Ship controls:");
                        Instructions.NewLine("Space = Fire");
                        Instructions.NewLine("Left/Right = Move");
                        Instructions.NewLine("Up = Hyperdrive");
                        Instructions.NewLine("Down = Toggle S-foils");
                        Instructions.NewLine();
                        Instructions.NewLine("System controls:");
                        Instructions.NewLine("PageUp = Faster");
                        Instructions.NewLine("PageDown = Slower");
                        Instructions.NewLine("Enter = Instructions");
                        Instructions.NewLine("Esc = Quit");
                        Instructions.NewLine();
                        Instructions.NewLine("Lock S-foils in attack position for");
                        Instructions.NewLine("faster blasting & slower flying.");
                        Instructions.NewLine();
                        Instructions.NewLine("Defeat all enemies for navicomputer bonus.");
                        Instructions.NewLine();
                        Instructions.NewLine("Hit = 1 point");
                        Instructions.NewLine("Kill = 2 points");
                        Instructions.NewLine("Score multiplier for higher altitude hits.");
                        Instructions.NewLine();
                        Instructions.NewLine("Press Enter to resume game.");
                    }
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
                            stars.SetHyperspace(false);
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

                        // hud
                        string ShieldMarkers = "";
                        if (player.HP > 0) { ShieldMarkers = new String('$', player.HP - 1); }
                        Screen.TryWrite(new Point(1, player.XY.iY + 1), ShieldMarkers + ' ');
                        string ShotMarkers = new string(' ', player.Missiles.Count) + new String('|', player.MissileCapacity - player.Missiles.Items.Count);
                        Screen.TryWrite(new Point(Screen.Width - ShotMarkers.Length - 1, player.XY.iY + 1), ShotMarkers);

                    }

                    if (player.Alive) { player.Animate(); }
                    if (player.Active) { player.Activate(); }
                    Score += wave.CollectScore();

                }


                // check input keys and prioritize some keys
                while (Console.KeyAvailable)
                {
                    ConsoleKeyInfo k = Console.ReadKey(true);
                    if (k.Key == ConsoleKey.UpArrow)
                    {
                        keybuffer.Clear();
                        keybuffer.Add(k);
                    }
                    else if (k.Key == ConsoleKey.Escape)
                    {
                        keybuffer.Clear();
                        keybuffer.Add(k);
                    }
                    else if (k.Key == ConsoleKey.LeftArrow || k.Key == ConsoleKey.RightArrow)
                    {
                        keybuffer = new List<ConsoleKeyInfo>(keybuffer.RemoveAll(x => x.Key == ConsoleKey.LeftArrow || x.Key == ConsoleKey.RightArrow));
                        keybuffer.Insert(0, k);
                    }
                    else if (k.Key == ConsoleKey.PageUp || k.Key == ConsoleKey.PageDown)
                    {
                        keybuffer = new List<ConsoleKeyInfo>(keybuffer.RemoveAll(x => x.Key == ConsoleKey.PageUp || x.Key == ConsoleKey.PageDown));
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

                            if (HyperdriveMode == eHyperdriveMode.Unused && !wave.Completed() && player.Alive)
                            {
                                if (Waves.IndexOf(wave) < Waves.Count - 1)
                                {
                                    stars.SetHyperspace(true);
                                    player.Trajectory.Run = 0;
                                    player.Missiles.TerminateAll();
                                    HyperdriveMode = eHyperdriveMode.Engaged;
                                    Easy.Clock.StartTimer();
                                }
                                else
                                {
                                    Scroller.NewLine("It's not my fault!");
                                    Scroller.NewLine("They told me they fixed it!");
                                }
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
                            player.GoLeft();
                            break;
                        case ConsoleKey.RightArrow:
                            player.GoRight();
                            break;
                        case ConsoleKey.DownArrow:
                            player.ToggleFlightMode();
                            break;
                        case ConsoleKey.Spacebar:
                            if (HyperdriveMode != eHyperdriveMode.Engaged) { player.Fire(); }
                            break;
                        case ConsoleKey.Escape:
                            GetTheFkOut = true;
                            break;
                        case ConsoleKey.Enter:
                            Paused = !Paused;
                            if (!Paused) { Instructions.TerminateAll(); }
                            break;
                    }

                }

                // display messages
                if (!Instructions.Empty) { Instructions.Animate(); } else { Scroller.Animate(); }
                if (player.Alive)
                {
                    if (wave.Completed() && !ClosingWordsStated)
                    {
                        ClosingWordsStated = true;
                        if (HyperdriveMode == eHyperdriveMode.Disengaged)
                        {
                            Scroller.NewLine("Traveling through hyperspace");
                            Scroller.NewLine("ain't like dusting crops, boy.");
                            hyperbonus.Reset();
                        }
                        else
                        {
                            if (wave.VictoryMessage == "") { Scroller.NewLine("Wave cleared."); }
                            else { Scroller.NewLine(wave.VictoryMessage); }
                            Scroller.NewLine("+" + (hyperbonus.Value * 10) + " navicomputer Fibonacci bonus.");
                            Score += hyperbonus.Value * 10;
                            hyperbonus.Increment();
                        };
                    }
                }
                else
                {
                    if (!ClosingWordsStated)
                    {
                        ClosingWordsStated = true;
                        Scroller.NewLine("G A M E   O V E R");
                        Scroller.NewLine(3);
                        Scroller.NewLine("They came from behind!");
                    }

                    if (wave.Completed() && !HeroBonusGiven)
                    {
                        int deadherobonus = 500;
                        Scroller.NewLine(3);
                        Scroller.NewLine("That");
                        Scroller.NewLine("was");
                        Scroller.NewLine("awesome.");
                        Scroller.NewLine(3);
                        Scroller.NewLine("+" + deadherobonus + " Dead hero bonus.");
                        Score += deadherobonus;
                        HeroBonusGiven = true;
                    }
                }

                Console.Title = "Score: " + Score;

                // throttle the cpu
                if (!GetTheFkOut)
                {
                    if (HyperdriveMode == eHyperdriveMode.Engaged) { Easy.Clock.FpsThrottle(100); }
                    else { Easy.Clock.FpsThrottle(FramesPerSecond); }
                }

            } while (!GetTheFkOut && (!Scroller.Empty || (player.Active && !wave.Completed())));

            if (!player.Alive || GetTheFkOut) { break; }

        }

        return Score;
    }
}