using System;
using System.Collections.Generic;

public class Armada
{

    #region " Properties "

    int _MaxShips;
    int _SelectionRange = 1;

    List<Ship> _ships = new List<Ship>();
    AsciiEngine.SpriteField _explosions = new AsciiEngine.SpriteField();

    public List<Ship> Ships { get { return this._ships; } }

    #endregion

    #region " Ship Creation "

    void IncreaseShipLimit() { this._MaxShips++; }

    public void Spawn()
    {
        while (_ships.Count < this._SelectionRange / 3 || _ships.Count == 0)   // this._MaxShips)
        {
            Random r = new Random();
            int selection = r.Next(this._SelectionRange);
            if (selection < 5) { _ships.Add(new Ship(Ship.eShipType.Fighter)); }
            else if (selection < 10) { _ships.Add(new Ship(Ship.eShipType.Bomber)); }
            else if (selection < 15) { _ships.Add(new Ship(Ship.eShipType.Interceptor)); }
            else if (selection < 20) { _ships.Add(new Ship(Ship.eShipType.Vader)); }
            else if (selection < 30) { _ships.Add(new Ship(Ship.eShipType.Vader)); }
            else { _ships.Add(new Ship(Ship.eShipType.Fighter)); }


        }
    }

    #endregion

    #region " Movement and Drawing "

    #region " Ship Damage "

    void Sweep()
    {
        foreach (Ship s in this._ships.FindAll(x => !x.Alive))
        {
            s.Hide();
            this._explosions.Sprites.AddRange(s.Debris);
            this._ships.Remove(s);

            Random r = new Random();
            if (r.NextDouble() < .2) { this.IncreaseShipLimit(); }
            this._SelectionRange++;
        }
    }

    public void HurtShip(Ship s, int hp)
    {
        s.Hurt(hp);
        this._explosions.Sprites.AddRange(s.Sparks);
    }

    #endregion


    public void Animate()
    {
        foreach (Ship ship in _ships.FindAll(x => x.Alive)) { ship.Animate(); }
        this._explosions.Animate(); // move explosions
        this.Sweep(); // remove dead ships

    }

    #endregion

    #region " Constructor "
    public Armada(int maxfighters)
    {
        this._MaxShips = maxfighters;
    }

    #endregion

}