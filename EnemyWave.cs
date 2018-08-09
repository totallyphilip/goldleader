using System.Collections.Generic;

public class EnemyWave : AsciiEngine.Sprites.Swarm

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

    string WelcomeMessage;
    public string VictoryMessage;
    public int FrameCounter = 0;
    public int AirTrafficMax;
    bool AttackRunStarted = false;
    public void StartAttackRun() { this.AttackRunStarted = true; }
    public List<EnemyDefinition> Generator = new List<EnemyDefinition>();
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

    public bool HasWelcomeMessage { get { return this.WelcomeMessage != ""; } }

    public string PopWelcomeMessage()
    {
        string s = this.WelcomeMessage;
        this.WelcomeMessage = "";
        return s;
    }

    protected override void OnAnimated()
    {
        if (this.AttackRunStarted)
        {
            this.FrameCounter++; // this will eventually overflow if the wave never ends but i don't care
        }
    }

    public EnemyWave(int max, string welcome, string victory, bool upgradeblasters)
    {
        this.WelcomeMessage = welcome;
        this.VictoryMessage = victory;
        this.AirTrafficMax = max;
        this.WeaponsUpgrade = upgradeblasters;
    }

    public bool Completed()
    {
        if (this.Escaped) { return true; }
        else { return this.Generator.Count == 0 && !this.Alive; }
    }


    protected override void Spawn()
    {
        if (this.AttackRunStarted)
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