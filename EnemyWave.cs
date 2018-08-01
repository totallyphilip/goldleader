using System.Collections.Generic;

public class EnemyWave : AsciiEngine.Sprites.Swarm

{

    public class Squadron
    {
        public Enemy.eEnemyType EnemyType;
        public int Count;
        public bool IsSquad = false;

        public Squadron(Enemy.eEnemyType enemytype, int count) : this(enemytype, count, false) { }

        public Squadron(Enemy.eEnemyType enemytype, int count, bool squad)
        {
            this.EnemyType = enemytype;
            this.Count = count;
            this.IsSquad = squad;
        }
    }

    public int AirTrafficMax;
    int UnflownCount = 0;
    bool AttackRunStarted = false;
    public void StartAttackRun() { this.AttackRunStarted = true; }
    public List<Squadron> Fleet = new List<Squadron>();
    List<Enemy> IncomingShips = new List<Enemy>();
    List<Enemy> SquadLeaders = new List<Enemy>();
    public string WinMessage;
    public bool Infinite = false;
    public bool WeaponsUpgrade = false;
    public bool Escaped = false;

    protected override void OnRefreshed()
    {
        foreach (Enemy bg in this.Items)
        {
            bg.Sparks.Refresh();
            bg.Debris.Refresh();
            bg.Messages.Refresh();
            bg.Missiles.Refresh();
        }
    }

    public void ExitHyperspace() { this.Escaped = true; }


    public EnemyWave(bool moreguns)
    {
        this.AirTrafficMax = 8;
        this.Infinite = true;
        this.WeaponsUpgrade = moreguns;
    }

    public EnemyWave(int max, string win, bool moreguns)
    {
        this.AirTrafficMax = max;
        this.WinMessage = win;
        this.WeaponsUpgrade = moreguns;
    }

    public bool Completed()
    {
        if (this.Infinite) { return false; }
        else if (this.Escaped) { return true; }
        else { return this.UnflownCount == 0 && !this.Alive; }
        //        else { return this.UnflownCount == 0 && IncomingShips.FindAll(x => x.Alive).Count == 0; }
    }

    public void CreateIncomingFleet()
    {
        // create the requested number of ships for each ship type
        foreach (Squadron squad in Fleet)
        {
            for (int i = 0; i < squad.Count; i++)
            {
                Enemy bg = new Enemy(squad.EnemyType);
                IncomingShips.Add(bg);
                UnflownCount++;
                if (squad.IsSquad) { SquadLeaders.Add(bg); }
            }
        }
    }

    protected override void Spawn()
    {
        if (this.AttackRunStarted)
        {
            if (this.Infinite)
            {
                if (this.Items.Count < this.AirTrafficMax)
                {
                    System.Array values = System.Enum.GetValues(typeof(Enemy.eEnemyType));
                    Enemy.eEnemyType someone = (Enemy.eEnemyType)values.GetValue(Easy.Abacus.Random.Next(values.Length));
                    this.Items.Add(new Enemy(someone));
                }
            }
            else
            {
                if (this.Items.Count < this.AirTrafficMax && this.IncomingShips.FindAll(x => !x.Flown).Count > 0)
                {
                    // pick a ship at random
                    Enemy randbg = IncomingShips.Find(x => !x.Flown && Easy.Abacus.RandomTrue);
                    if (randbg != null)
                    {
                        randbg.Flown = true;
                        this.Items.Add(randbg);
                        UnflownCount--;


                        if (SquadLeaders.Exists(x => x.Equals(randbg)))
                        {
                            Enemy follower;
                            follower = new Enemy(Enemy.eEnemyType.HeavyFighter);
                            follower.Leader = randbg;
                            follower.Trail.Add(randbg.XY.Clone(-1 * randbg.Width, 0));
                            this.Add(follower);
                            follower = new Enemy(Enemy.eEnemyType.HeavyFighter);
                            follower.Leader = randbg;
                            follower.Trail.Add(randbg.XY.Clone(randbg.Width, 0));
                            this.Add(follower);
                        }
                    }
                }
            }
        }
    }
}