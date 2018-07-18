using System;
using System.Collections.Generic;

public class Armada
{

    #region " Properties "

    int _MaxFighters;

    List<Ship> ships = new List<Ship>();

    #endregion

    #region " Ship Creation "

    void RaiseFighterLimit() { this._MaxFighters++; }

    public void Spawn()
    {
        while (ships.Count < this._MaxFighters)
        {
            ships.Add(new Ship(Ship.eShipType.Fighter));
            ships.Add(new Ship(Ship.eShipType.Fighter));
            ships.Add(new Ship(Ship.eShipType.Fighter));
            ships.Add(new Ship(Ship.eShipType.Bomber));
            ships.Add(new Ship(Ship.eShipType.Bomber));
            ships.Add(new Ship(Ship.eShipType.Vader));
            ships.Add(new Ship(Ship.eShipType.Squadron));
            ships.Add(new Ship(Ship.eShipType.Interceptor));
        }
    }

    #endregion

    #region " Movement and Drawing "

    public void Animate()
    {
        foreach (Ship ship in ships) { ship.Animate(); }
    }

    #endregion

    #region " Constructor "
    public Armada(int maxfighters)
    {
        this._MaxFighters = maxfighters;
    }

    #endregion

}