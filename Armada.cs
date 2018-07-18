using System;
using System.Collections.Generic;

public class Armada
{

    #region " Properties "

    int _MaxFighters;

    List<Ship> _ships = new List<Ship>();
    AsciiEngine.SpriteField _explosions = new AsciiEngine.SpriteField();

    #endregion

    #region " Ship Creation "

    void RaiseFighterLimit() { this._MaxFighters++; }

    public void Spawn()
    {
        while (_ships.Count < this._MaxFighters)
        {
            _ships.Add(new Ship(Ship.eShipType.Fighter));
            _ships.Add(new Ship(Ship.eShipType.Fighter));
            _ships.Add(new Ship(Ship.eShipType.Fighter));
            _ships.Add(new Ship(Ship.eShipType.Bomber));
            _ships.Add(new Ship(Ship.eShipType.Bomber));
            _ships.Add(new Ship(Ship.eShipType.Vader));
            _ships.Add(new Ship(Ship.eShipType.Squadron));
            _ships.Add(new Ship(Ship.eShipType.Interceptor));
        }
    }

    #endregion

    #region " Movement and Drawing "

    public void Animate()
    {
        foreach (Ship ship in _ships) { ship.Animate(); }
        this._explosions.Animate();
    }

    #endregion

    #region " Constructor "
    public Armada(int maxfighters)
    {
        this._MaxFighters = maxfighters;
    }

    #endregion

    #region " Testing "

    public void test()
    {
        foreach (Ship s in this._ships)
        {
            s.Hurt();
        }

        foreach (Ship s in this._ships.FindAll(x => !x.Alive))
        {
            s.Hide();
            foreach (AsciiEngine.Sprite exp in s.Debris)
            {
                this._explosions.Sprites.Add(exp);
            }
            this._ships.Remove(s);
        }



    }
    #endregion

}