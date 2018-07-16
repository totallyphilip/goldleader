using System;

   class Enemy {

        enum ShipType {Fighter, Vader, Bomber};

        string _Ascii;
        int _X;
        int _Y;
        int _HP;
        int _Reward;
        int _Drift;
        int _TopBounceLine;
        int _BottomBounceLine;
        int _Gravity;

        public int HP {
            get {return _HP;}
        }

        public void Show() {
            if (! (this.FellOffTheScreen || TUI.PastEdge(_X,_Ascii))) {
                Console.SetCursorPosition(_X,_Y);
                Console.Write(_Ascii);
            }
        }

        public void Hide() {
            if (! (this.FellOffTheScreen || TUI.PastEdge(_X,_Ascii))) {
                Console.SetCursorPosition(_X,_Y);
                Console.Write(TUI.StringRepeat(_Ascii.Length,' '));
            }
        }

        public bool FellOffTheScreen {
            get {return _Y > TUI.BottomEdge || _Y < 0;}
        }

        public void Descend() {
            _Y+=_Gravity;
        }

        public void Move() {
            this.Hide();
            Random r = new Random();
            if (TUI.PastEdge(_X,_Drift,_Ascii)) {
                _Drift *= -1; // reverse direction
                _Y += _Gravity; // lower altitude
            } else {
                if (r.Next(TUI.BottomEdge-this._Y) == 0) {this.Descend();}
            }
            if (_Y < _TopBounceLine) { _Gravity = 1;}
            if (_Y > _BottomBounceLine) { _Gravity = -1;}
            if (this.FellOffTheScreen) {_Y = 0;}
            _X += _Drift;
            this.Show();
                        
        }


        public Enemy(){

            Random r = new Random();

            switch (r.Next(3))
            {
                case 0:
                    _Ascii = "|-o-|";
                    _HP=1;
                    _TopBounceLine = 0;
                    _BottomBounceLine = TUI.BottomEdge+1; // never bounce
                    break;
                case 1:
                    _Ascii = "[-0-]";
                    _HP=3;
                    _TopBounceLine = TUI.BottomEdge / 2;
                    _BottomBounceLine = TUI.BottomEdge-1;
                    break;
                case 2:
                    _Ascii = "[-0-o-]";
                    _HP = 2;
                    _TopBounceLine = TUI.BottomEdge/2;
                    _BottomBounceLine = _TopBounceLine + _TopBounceLine/2;
                    break;
            }

            _Y = 0;
            _Gravity = 1;
             _Reward = _HP;
            _X = r.Next(TUI.LeftEdge+5,TUI.RightEdge-5);
            if (r.Next(2)==0) {
                _Drift = 1;
            }
            else {_Drift = -1;}
        }
    }