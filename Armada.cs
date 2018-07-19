using System;
using System.Collections.Generic;

public class Armada
{

    #region " Properties "

    int _MaxShips;

    List<Ship> _ships = new List<Ship>();
    AsciiEngine.SpriteField _explosions = new AsciiEngine.SpriteField();

    public List<Ship> Ships { get { return this._ships; } }

    #endregion

    #region " Ship Creation "

    void IncreaseShipLimit() { this._MaxShips++; }

    public void Spawn()
    {
        while (_ships.Count < this._MaxShips)
        {
            Random r = new Random();
            switch (r.Next(5))
            {
                case 0:
                    _ships.Add(new Ship(Ship.eShipType.Fighter));
                    break;
                case 1:
                    _ships.Add(new Ship(Ship.eShipType.Bomber));
                    break;
                case 2:
                    _ships.Add(new Ship(Ship.eShipType.Interceptor));
                    break;
                case 3:
                    _ships.Add(new Ship(Ship.eShipType.Vader));
                    break;
                case 4:
                    _ships.Add(new Ship(Ship.eShipType.Squadron));
                    break;

            }
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
            if (r.NextDouble() < .25) { this.IncreaseShipLimit(); }
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
        foreach (Ship ship in _ships) { ship.Animate(); } // move ships
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