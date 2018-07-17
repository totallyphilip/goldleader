using System;
using System.Collections.Generic;

public class EnemyFleet
{

    #region " Sub Classes "

    class Fighter
    {

        #region " Sub Classes "

        class FlyZone
        {

            int _Top;
            int _Bottom;
            int _Left;
            int _Right;

            #region " Constructor "

            public FlyZone(int top, int bottom) : this(top, bottom, Textify.LeftEdge, Textify.RightEdge) { }

            public FlyZone(int top, int bottom, int left, int right)
            {
                _Top = top;
                _Left = left;
                _Bottom = bottom;
                _Right = right;

            }

            #endregion

        }

        #endregion

        #region " Enum "

        enum eFighter { Fighter, Vader, Bomber };

        #endregion

        #region " Properties "

        int _X;
        int _Y;
        string _Ascii;
        int _HP;

        FlyZone flyzone;

        int Width
        {
            get { return _Ascii.Length; }
        }

        #endregion

        #region " Methods "

        void MoveTo(int x, int y)
        {
            _X = x;
            _Y = y;
        }

        #endregion

        #region " Constructor "

        public Fighter()
        {
            Random r = new Random();
            if (r.Next(2) == 0) { this.MoveTo(Textify.LeftEdge - this.Width, -1); }
            flyzone = new FlyZone(0, Textify.BottomEdge);
        }
        #endregion

    }

    #endregion

    #region " Properties "

    List<Fighter> fighters = new List<Fighter>();

    #endregion

    #region " Constructor "
    public EnemyFleet()
    {

    }

    #endregion

}