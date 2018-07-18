using System;
using System.Collections.Generic;

public class Armada
{

    #region " Properties "

    int _MaxFighters;

    List<Ship> fighters = new List<Ship>();

    #endregion

    #region " Methods "

    void RaiseFighterLimit() { this._MaxFighters++; }

    public void Spawn()
    {
        while (fighters.Count < this._MaxFighters)
        {
            fighters.Add(new Ship(Ship.eShipType.Vader));
        }
    }

    #endregion

    #region " Constructor "
    public Armada(int maxfighters)
    {
        this._MaxFighters = maxfighters;
    }

    #endregion

}