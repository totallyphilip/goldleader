using AsciiEngine;
using Easy;
using System;
using System.Collections.Generic;

public class Armada
{

    #region " Properties "

    int _RandomShipSelectionRange = 1;

    List<Ship> _ships = new List<Ship>();
    SpriteField _explosions = new AsciiEngine.SpriteField();

    public List<Ship> Ships { get { return this._ships; } }

    #endregion

    #region " Ship Creation "

    public void Spawn()
    {
        while (_ships.Count < this._RandomShipSelectionRange / 3 || _ships.Count == 0)
        {
            int selection = Easy.Numbers.Random.Next(this._RandomShipSelectionRange);
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
        // first blow them up
        foreach (Ship s in this._ships.FindAll(x => !x.Alive && !x.Exploded))
        {
            s.Hide();
            this._explosions.Sprites.AddRange(s.Debris);
            s.Exploded = true;

            this._RandomShipSelectionRange++;
        }
        // remove them once all their missiles are gone
        this._ships.RemoveAll(x => !x.Alive && !x.Firing);

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
        foreach (Ship ship in _ships.FindAll(x => x.Firing)) { ship.MissileField.Animate(); }
        this._explosions.Animate(); // move explosions
        this.Sweep(); // remove dead ships

    }

    #endregion

    #region " Constructor "
    public Armada(int maxfighters) { }

    #endregion

}