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
    bool Attack = false;
    public void StartAttackRun() { this.Attack = true; }
    public List<Squadron> Fleet = new List<Squadron>();
    List<BadGuy> IncomingShips = new List<BadGuy>();
    public string ReadyMessage;
    public string WinMessage;
    public bool Congratulated = false;
    public string LoseMessage;
    public bool Humiliated = false;
    public bool Infinite = false;

    public EnemyWave(string ready, string lose)
    {
        this.AirTrafficMax = 20;
        this.ReadyMessage = ready;
        this.LoseMessage = lose;
        this.Infinite = true;
    }

    public EnemyWave(int max, string ready, string win, string lose)
    {
        this.AirTrafficMax = max;
        this.ReadyMessage = ready;
        this.WinMessage = win;
        this.LoseMessage = lose;
    }

    public bool WaveDefeated()
    {
        if (this.Infinite) { return false; }
        else { return IncomingShips.FindAll(x => x.Flown).Count > 0 && this.Empty; }
    }

    public void CreateIncomingFleet()
    {
        // create the requested number of ships for each ship type
        foreach (Squadron squad in Fleet)
        {
            for (int i = 0; i < squad.Count; i++)
            {
                IncomingShips.Add(new BadGuy(squad.BadGuyType));
            }
        }
    }

    protected override void Spawn()
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
            if (this.Attack && this.Items.Count < this.AirTrafficMax && this.IncomingShips.FindAll(x => !x.Flown).Count > 0)
            {
                // pick a ship at random
                BadGuy randbg = IncomingShips.Find(x => !x.Flown && Easy.Abacus.RandomTrue);
                if (randbg != null)
                {
                    randbg.Flown = true;
                    this.Items.Add(randbg);
                }
            }
        }

    }

}