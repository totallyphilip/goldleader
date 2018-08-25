using UnicodeEngine;
using UnicodeEngine.Fx;
using UnicodeEngine.Grid;
using UnicodeEngine.Sprites;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

internal class CharSet
{
    public static char Missile { get { return '|'; } }
    public static char Shield { get { return Character.DiamondLight; } }
    public static char Torpedo { get { return Character.TriangleUpSolid; } }
    public static char Smoke { get { return Character.ShadeLight; } }
    public static char Damage { get { return Character.HyphenDoubleOblique; } }
    public static char Debris { get { return 'x'; } }
    // power ups
    public static char AirStrike { get { return '#'; } }
    public static char Jump { get { return '^'; } }
    public static char Shrapnel { get { return '*'; } }
}

public class UnicodeWars
{
    bool QuitFast = false;
    int FramesPerSecond = 10;
    bool PlayAgain;
    Galaxy Stars;
    int TimeLimit = 90;


    public UnicodeWars() { this.PlayAgain = true; }
    public UnicodeWars(bool b) { PlayAgain = b; }

    public int TryPlay(int HighScore)
    {
        int oldwidth = Console.WindowWidth;
        int oldheight = Console.WindowHeight;
        Screen.TryInitializeScreen(50, 40, false);
        int Score = 0;
        Stars = new Galaxy();
        Score = this.MainLoop(HighScore);
        Screen.TryInitializeScreen(oldwidth, oldheight, false);
        Console.CursorVisible = true;
        return Score;
    }

    int MainLoop(int HighScore)
    {
        Easy.Keyboard.EatKeys();
        int Score = 0;
        do
        {
            if (!QuitFast) { Attract(); }
            if (!QuitFast) { Score = PlayTheGame(HighScore); }
        } while (!QuitFast && this.PlayAgain);
        return Score;
    }

    SqlConnection DbConnection()
    {
        string constr = "user id=dbTest;password=baMw$CAQ5hnlxjCTYJ0YP;server=sql01\\dev01;Trusted_Connection=no;database=PwrightSandbox;connection timeout=5";
        //MySqlConnection qConn = new MySqlConnection("server=192.168.242.10;user=foo;database=GameData;password=12345");
        //MySqlConnection("server=192.168.242.10;user=foo;database=GameData;password=12345");
        return new SqlConnection(constr);
    }

    void Attract()
    {
        Console.Clear();
        Swarm DemoEnemies = new Swarm();

        #region " Text "

        List<string> Messages = new List<string>();

        Messages.Add("U N I C O D E   W A R S");
        Messages.Add("");
        foreach (Enemy.eEnemyType shiptype in (Enemy.eEnemyType[])System.Enum.GetValues(typeof(Enemy.eEnemyType)))
        {
            Enemy bg = new Enemy(shiptype,true);
            DemoEnemies.Items.Add(bg);
            Messages.Add(new string(bg.Text) + " " + Enum.GetName(typeof(Enemy.eEnemyType), shiptype) + " (" + bg.HitPoints + " HP)");
        }
        Messages.Add("");
        Messages.Add("Press Esc to Quit");
        Messages.Add("Press Space to Begin");
        Messages.Add("");
        Messages.Add("");
        Messages.Add("");

        #endregion


        Scroller Scroller = new Scroller(2, Screen.Height / 2, .25);
        do
        {
            Console.CursorVisible = false;

            if (DemoEnemies.Empty)
            {
                foreach (Enemy.eEnemyType shiptype in (Enemy.eEnemyType[])System.Enum.GetValues(typeof(Enemy.eEnemyType)))
                {
                    DemoEnemies.Items.Add(new Enemy(shiptype,true));
                }
            }

            if (Scroller.Empty)
            {

                SqlConnection dbConn = DbConnection();
                try
                {
                    dbConn.Open();
                    SqlCommand oCommand = new SqlCommand("dbo.GetScores", dbConn);
                    oCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    oCommand.Parameters.AddWithValue("@gameid", 1);
                    SqlDataReader dbReader = oCommand.ExecuteReader();

                    if (dbReader.HasRows)
                    {
                        Scroller.NewLine("TOP 20 SCORES");
                        Scroller.NewLine();
                        while (dbReader.Read())
                        {
                            Scroller.NewLine(
                                 dbReader["Score"].ToString()
                                + " " + dbReader["Signature"].ToString()
                            );
                        }
                        Scroller.NewLine();
                    }
                }
                catch { }

                try { dbConn.Close(); } catch { }

                foreach (string s in Messages)
                {
                    Scroller.NewLine(s);
                }
            }
            Stars.Animate();
            DemoEnemies.Animate();
            UnicodeEngine.Sprites.Static.Swarms.Animate();
            UnicodeEngine.Sprites.Static.Sprites.Animate();
            Scroller.Animate();
            Easy.Clock.FpsThrottle(8);

            if (Easy.Abacus.Random.NextDouble() < .05)
            {
                int victim = Easy.Abacus.Random.Next(0, DemoEnemies.Count);
                if (DemoEnemies.Items[victim].Alive) { DemoEnemies.Items[victim].OnHit(-1); }
            }

        } while (!Console.KeyAvailable);

        QuitFast = Console.ReadKey(true).Key == ConsoleKey.Escape;

    }

    enum eHyperdriveMode { Unused, Engaged, Disengaged };

    int PlayTheGame(int HighScore)
    {

        Console.Clear();

        // the user
        Player player = new Player();
        int Score = 0;

        // power ups
        PowerUp powerup = null;
        int BonusPoints = 0;
        int FrameCounter = 0;

        // hyperdrive
        eHyperdriveMode HyperdriveMode = eHyperdriveMode.Unused;
        Easy.Abacus.Fibonacci hyperbonus = new Easy.Abacus.Fibonacci();

        // misc
        bool Paused = false;
        bool HeroBonusGiven = false;
        Easy.Clock.Timer timer = new Easy.Clock.Timer();

        #region  " Waves "

        // define the waves of bad guys

        List<EnemyWave> Waves = new List<EnemyWave>();

        EnemyWave Wave;

        Wave = new EnemyWave(100, "", "Great, kid! Don't get cocky.", false);
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
        Wave.Generator.Add(new EnemyWave.EnemyDefinition(Enemy.eEnemyType.Fighter, 30));
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
        Scroller Scroller = new Scroller(2, Screen.Height / 3, .25);
        Scroller Instructions = new Scroller(2, Screen.Height / 3, .25, ConsoleColor.Green, ConsoleColor.DarkGray);
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

            if (Waves.IndexOf(wave) == Waves.Count - 1)
            {
                player.HitPoints += 4;
                Scroller.NewLine("Final wave!");
                Scroller.NewLine("Deflector shield boosted.");
                Scroller.NewLine("May the force be with you.");
            }
            else
            {
                Scroller.NewLine("Wave " + (Waves.IndexOf(wave) + 1));
                Scroller.NewLine("Deflector shield " + (Convert.ToDouble(player.HitPoints - 1) / (player.DefaultHitPoints - 1)) * 100 + "% charged.");
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

            int FramesUntilPowerup = Easy.Abacus.Random.Next(100, 150);

            bool AttackRunStarted = false;
            //timer.Pause();

            do
            {

                if (!AttackRunStarted)
                {
                    if (Scroller.Empty) // wait until wave intro messages are gone
                    { 
                        wave.StartAttackRun();
                        timer.Start();
                        AttackRunStarted = true;
                    }

                }


                Console.CursorVisible = false; // windows turns the cursor back on when restoring from minimized window

                // power ups
                if (FrameCounter > FramesUntilPowerup && powerup == null && !wave.Completed())
                {
                    PowerUp.ePowerUpType pt = Easy.Abacus.RandomEnumValue<PowerUp.ePowerUpType>();
                    powerup = new PowerUp(pt);
                    //powerup = new PowerUp(PowerUp.ePowerUpType.Missiles); // to force a powerup choice
                    FramesUntilPowerup = Easy.Abacus.Random.Next(200, 300);
                    FrameCounter = 0;
                }

                // animate
                Stars.Animate();
                FrameCounter++;



                if (Paused)
                {
                    player.Refresh();
                    UnicodeEngine.Sprites.Static.Swarms.Refresh();
                    player.Missiles.Refresh();
                    wave.Refresh();
                    if (Instructions.Empty)
                    {
                        Scroller.HideAll();
                        Instructions.NewLine("Press Enter to resume game.");
                        Instructions.NewLine();
                        Instructions.NewLine("Ship controls:");
                        Instructions.NewLine("Space = Fire Blasters");
                        Instructions.NewLine("Control-Space = Fire Torpedo");
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
                        Instructions.NewLine("Lock S-foils in attack position to");
                        Instructions.NewLine("divert power from thrusters to blasters.");
                        Instructions.NewLine();
                        Instructions.NewLine("Defeat all enemies for navicomputer bonus.");
                        Instructions.NewLine();
                        Instructions.NewLine("Hit = 1 point X altitude factor");
                        Instructions.NewLine("Kill = 2 points X altitude factor");
                        Instructions.NewLine();
                        Instructions.NewLine("Press Enter to resume game.");
                    }
                }
                else
                {

                    UnicodeEngine.Sprites.Static.Swarms.Animate();
                    UnicodeEngine.Sprites.Static.Sprites.Animate();

                    if (HyperdriveMode == eHyperdriveMode.Engaged)
                    {
                        Stars.Animate();
                        if (Easy.Clock.Elapsed(3))
                        {
                            HyperdriveMode = eHyperdriveMode.Disengaged;
                            wave.ExitHyperspace();
                            Stars.SetHyperspace(false);
                        }
                    }
                    else if (HyperdriveMode == eHyperdriveMode.Unused)
                    {

                        // power ups
                        if (powerup != null)
                        {

                            bool hit = Sprite.Collided(player, powerup);
                            powerup.Animate();
                            if (hit || Sprite.Collided(player, powerup))
                            {
                                int points = powerup.Points;
                                switch (powerup.PowerUpType)
                                {
                                    case PowerUp.ePowerUpType.Points:
                                        BonusPoints += 10;
                                        points = BonusPoints;
                                        break;
                                    case PowerUp.ePowerUpType.Shields:
                                        break;
                                    case PowerUp.ePowerUpType.Missiles:
                                        player.FireBloom();
                                        break;
                                    case PowerUp.ePowerUpType.Airstrike:
                                        player.FireAirStrike();
                                        break;
                                    case PowerUp.ePowerUpType.Jump:
                                        player.DropIn(.3);
                                        break;
                                    case PowerUp.ePowerUpType.Torpedo:
                                        player.TorpedosLocked += 1;
                                        break;
                                }
                                Score += points;
                                ScoreUp(points, player.XY);
                            }

                            if (!powerup.Alive) { powerup = null; }

                        }

                        // check if anybody shot anybody
                        wave.CheckCollisions(player.Missiles);
                        wave.Animate();
                        wave.CheckCollisions(player.Missiles);

                        // give the player a break by not checking if they ran into a missile
                        foreach (Enemy bg in wave.Items) { bg.Missiles.CheckCollision(player); }

                        // hud
                        Console.ForegroundColor = ConsoleColor.White;
                        string ShieldMarkers = "";
                        if (player.HitPoints > 0) { ShieldMarkers = new String(CharSet.Shield, player.HitPoints - 1); }
                        Screen.TryWrite(new Point(1, Screen.BottomEdge), ShieldMarkers + ' ');

                        try // this dies if the missile powerup is used
                        {
                            string ShotMarkers = new string(' ', player.Missiles.Count + player.Torpedos.Count) + new String('|', player.MissileCapacity - player.Missiles.Items.Count) + new string(CharSet.Torpedo, player.TorpedosLocked);
                            Screen.TryWrite(new Point(Screen.Width - ShotMarkers.Length - 1, Screen.BottomEdge), ShotMarkers);
                        }
                        catch
                        {
                            string ShotMarkers = new string(' ', player.MissileCapacity + player.Torpedos.Count) + new string(CharSet.Torpedo, player.TorpedosLocked);
                            Screen.TryWrite(new Point(Screen.Width - ShotMarkers.Length - 1, Screen.BottomEdge), ShotMarkers);
                        }

                        if (AttackRunStarted)
                        {

                            string secondsremaining = " " + (TimeLimit - timer.ElapsedSeconds).ToString("0") + " ";
                            if (TimeLimit - timer.ElapsedSeconds > 0) { Console.ForegroundColor = ConsoleColor.White; }
                            else { Console.ForegroundColor = ConsoleColor.Red; }

                            Screen.TryWrite(new Point((Screen.Width-secondsremaining.Length) / 2, Screen.BottomEdge), secondsremaining);
                        }

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
                                    Stars.SetHyperspace(true);
                                    powerup = null;
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
                            if (HyperdriveMode != eHyperdriveMode.Engaged)
                            {
                                if ((k.Modifiers & ConsoleModifiers.Control) != 0) { player.FireTorpedo(); }
                                else { player.Fire(); }

                            }
                            break;
                        case ConsoleKey.Escape:
                            QuitFast = true;
                            break;
                        case ConsoleKey.Enter:
                            Paused = !Paused;
                            if (Paused) { timer.Pause(); }
                            else { timer.Resume(); Instructions.TerminateAll(); }
                            break;
                    }

                }

                // display messages
                if (!Instructions.Empty) { Instructions.Animate(); } else { Scroller.Animate(); }
                if (player.Alive)
                {
                    if (wave.Completed() && !ClosingWordsStated)
                    {
                        timer.Pause();
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
                                int timebonus = TimeLimit - Easy.Abacus.Round( timer.ElapsedSeconds);
                            if (timebonus > 0)
                            {
                                Scroller.NewLine("+" + timebonus + " time bonus.");
                                Score += timebonus;
                            }
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
                if (!QuitFast)
                {
                    if (HyperdriveMode == eHyperdriveMode.Engaged) { Easy.Clock.FpsThrottle(100); }
                    else { Easy.Clock.FpsThrottle(FramesPerSecond); }
                }

            } while (!QuitFast && (!Scroller.Empty || (player.Active && !wave.Completed())));

            if (!player.Alive || QuitFast) { break; }

        }

        if (!QuitFast)
        {
            SqlConnection dbConn = DbConnection();
            try
            {
                dbConn.Open();

                // don't prompt for input until after we tried to open the database
                string initials = UnicodeEngine.Input.ArcadeInitials(new Point(Screen.Width / 2 - 2.5, Screen.Height / 2), 3);
                if (string.IsNullOrWhiteSpace(initials)) { initials = "???"; }

                SqlCommand oCommand = new SqlCommand("dbo.AddScore", dbConn);
                oCommand.CommandType = System.Data.CommandType.StoredProcedure;
                oCommand.Parameters.AddWithValue("@gameid", 1);
                oCommand.Parameters.AddWithValue("@score", Score);
                oCommand.Parameters.AddWithValue("@signature", initials);

                oCommand.ExecuteNonQuery();
            }
            catch { }

            try { dbConn.Close(); } catch { }
        }

        return Score;
    }

    static internal void ScoreUp(int points, Point xy)
    {
        if (points > 0) { Static.Sprites.Add(new Sprite(("+" + points.ToString()).ToCharArray(), xy.Clone(0, -1), new Trajectory(-.33, 0, 4), ConsoleColor.White)); }
        else if (points < 0) { Static.Sprites.Add(new Sprite((points.ToString()).ToCharArray(), xy.Clone(0, -1), new Trajectory(-.33, 0, 4), ConsoleColor.Red)); }
    }

}