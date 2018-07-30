using System.Collections.Generic;

public class EnemyWave : AsciiEngine.Sprites.Swarm

{

    public class Squadron
    {
        public BadGuy.eBadGuyType BadGuyType;
        public int Count;

        public Squadron(BadGuy.eBadGuyType badguytype, int count)
        {
            this.BadGuyType = badguytype;
            this.Count = count;
        }
    }

    public int AirTrafficMax;
    int UnflownCount = 0;
    bool AttackRunStarted = false;
    public void StartAttackRun() { this.AttackRunStarted = true; }
    public List<Squadron> Fleet = new List<Squadron>();
    List<BadGuy> IncomingShips = new List<BadGuy>();
    public string WinMessage;
    public bool Congratulated = false;
    public bool Humiliated = false;
    public bool Infinite = false;
    public bool WeaponsUpgrade = false;

    public void EnterHyperspace()
    {
        foreach (BadGuy bg in this.Items.FindAll(x => x.Alive))
        {
            bg.Debris.TerminateAll();
            bg.Sparks.TerminateAll();
            bg.Messages.TerminateAll();
            bg.Missiles.TerminateAll();
            bg.Hide();
        }
    }

    public void ExitHyperspace()
    {
        foreach (BadGuy bg in this.Items.FindAll(x => x.Alive))
        {
            bg.XY.dY = -1;
        }
    }


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

    public bool WaveDefeated()
    {
        if (this.Infinite) { return false; }
        else { return this.UnflownCount == 0 && IncomingShips.FindAll(x => x.Alive).Count == 0; }
    }

    public void CreateIncomingFleet()
    {
        // create the requested number of ships for each ship type
        foreach (Squadron squad in Fleet)
        {
            for (int i = 0; i < squad.Count; i++)
            {
                IncomingShips.Add(new BadGuy(squad.BadGuyType));
                UnflownCount++;
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
                    System.Array values = System.Enum.GetValues(typeof(BadGuy.eBadGuyType));
                    BadGuy.eBadGuyType someone = (BadGuy.eBadGuyType)values.GetValue(Easy.Abacus.Random.Next(values.Length));
                    this.Items.Add(new BadGuy(someone));
                }
            }
            else
            {
                if (this.Items.Count < this.AirTrafficMax && this.IncomingShips.FindAll(x => !x.Flown).Count > 0)
                {
                    // pick a ship at random
                    BadGuy randbg = IncomingShips.Find(x => !x.Flown && Easy.Abacus.RandomTrue);
                    if (randbg != null)
                    {
                        randbg.Flown = true;
                        this.Items.Add(randbg);
                        UnflownCount--;
                    }
                }
            }
        }
    }
}