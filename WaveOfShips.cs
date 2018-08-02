using System.Collections.Generic;

public class WaveOfShips : AsciiEngine.Sprites.Swarm

{

    public class EnemyDefinition
    {
        public Enemy.eEnemyType EnemyType;
        public int Count;
        public bool HasWingman = false;
        public Enemy.eEnemyType WingmanType;


        public EnemyDefinition(Enemy.eEnemyType enemytype, int count)
        {
            this.EnemyType = enemytype;
            this.Count = count;
        }

        public EnemyDefinition(Enemy.eEnemyType enemytype, int count, Enemy.eEnemyType wingmantype)
        {
            this.EnemyType = enemytype;
            this.Count = count;
            this.HasWingman = true;
            this.WingmanType = wingmantype;
        }
    }


    public int AirTrafficMax;
    bool AttackRunStarted = false;
    public void StartAttackRun() { this.AttackRunStarted = true; }
    public List<EnemyDefinition> Generator = new List<EnemyDefinition>();
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


    public WaveOfShips(bool moreguns)
    {
        this.AirTrafficMax = 8;
        this.Infinite = true;
        this.WeaponsUpgrade = moreguns;
    }

    public WaveOfShips(int max, string win, bool addblaster)
    {
        this.AirTrafficMax = max;
        this.WinMessage = win;
        this.WeaponsUpgrade = addblaster;
    }

    public bool Completed()
    {
        if (this.Infinite) { return false; }
        else if (this.Escaped) { return true; }
        else { return this.Generator.Count == 0 && !this.Alive; }
        //else { return this.UnflownCount == 0 && !this.Alive; }
        //        else { return this.UnflownCount == 0 && IncomingShips.FindAll(x => x.Alive).Count == 0; }
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
                if (this.Items.Count < this.AirTrafficMax && this.Generator.Count > 0)
                {
                    // get the next ship
                    Enemy bg = new Enemy(this.Generator[0].EnemyType);
                    this.Add(bg);
                    if (this.Generator[0].HasWingman)
                    {
                        Enemy wingman;
                        wingman = new Enemy(this.Generator[0].WingmanType);
                        wingman.Leader = bg;
                        wingman.Trail.Add(bg.XY.Clone(-1 * wingman.Width, 0));
                        this.Add(wingman);
                        wingman = new Enemy(this.Generator[0].WingmanType);
                        wingman.Leader = bg;
                        wingman.Trail.Add(bg.XY.Clone(bg.Width, 0));
                        this.Add(wingman);
                    }

                    // decrement the generator
                    this.Generator[0].Count--;
                    this.Generator.FindAll(x => x.Count < 1).ForEach(delegate (EnemyDefinition e) { this.Generator.Remove(e); });

                }
            }
        }
    }
}