using System;
using System.Collections.Generic;

public class StarField
{

    #region Sub Classes 

    class Star
    {

        #region " Vars / Properties" 

        int _X;
        int _Y;
        int _Delay;
        int _Parallax;

        public bool OffScreen
        {
            get { return _Y < 0 || _Y > TextMagic.BottomEdge; }
        }

        #endregion

        #region " Init "

        public Star(int Y)
        {
            Random r = new Random();
            _X = r.Next(TextMagic.LeftEdge, TextMagic.RightEdge);
            _Y = Y;
            if (r.Next(2) == 0)
            {
                _Parallax = 2;
            }
            else
            {
                _Parallax = 10;
            }
            _Delay = 0;
        }

        public Star() : this(-1) { }

        #endregion

        #region " Animation "

        void Hide()
        {
            if (!this.OffScreen)
            {
                Console.SetCursorPosition(_X, _Y);
                Console.Write(' ');
            }
        }

        void Show()
        {
            if (!this.OffScreen)
            {
                Console.SetCursorPosition(_X, _Y);
                Console.Write('.');
            }
        }

        public void Move()
        {
            if (_Delay == 0)
            {
                this.Hide();
                _Y++;
                _Delay = _Parallax;
                this.Show();
            }
            _Delay--;
        }

        #endregion
    }

    #endregion

    #region Vars / Properties

    List<Star> starfield = new List<Star>();

    #endregion

    #region Init

    public StarField()
    {
        for (int y = 0; y < TextMagic.BottomEdge; y++) { starfield.Add(new Star(y)); }
    }

    #endregion

    #region Animate

    public void Animate()
    {

        if (starfield.FindAll(x => x.OffScreen).Count > 0)
        {
            starfield.Remove(starfield.Find(x => x.OffScreen));
        }
        while (starfield.Count < TextMagic.BottomEdge)
        {
            starfield.Add(new Star());
        }
        foreach (var s in starfield)
        {
            s.Move();
        }

    }

    #endregion

}
