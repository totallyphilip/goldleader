using System;
using System.Threading;
using System.Collections.Generic;

namespace TieInvaders

    #region Classes
{
 
    class Misc {
        public static bool CoinFlip() {
            Random r = new Random();
            return r.Next(2) == 0;
        }

    }

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
            if (Misc.CoinFlip()) {
                _Drift = 1;
            }
            else {_Drift = -1;}
        }
    }

 /*    class StarField {

        #region Sub Classes 

        class Star {

            #region " Vars / Properties" 

            int _X;
            int _Y;
            int _Delay;
            int _Parallax;

            public bool OffScreen {
               get {return _Y < 0 || _Y > TUI.BottomEdge;}
            }

            #endregion

            #region " Init "
            
            public Star(int Y) {
                Random r = new Random();
                _X = r.Next(TUI.LeftEdge,TUI.RightEdge);
                _Y = Y;
                if (r.Next(2)==0) {
                    _Parallax = 2;
                } 
                else {
                    _Parallax = 10;
                }
                _Delay = 0;
            }

            public Star(): this(-1) {}

            #endregion

            #region " Animation "

            void Hide() {
                if (! this.OffScreen) {
                    Console.SetCursorPosition(_X,_Y);
                    Console.Write(' ');
                }
            }        

            void Show() {
                if (! this.OffScreen) {
                    Console.SetCursorPosition(_X,_Y);
                    Console.Write('.');
                }
            }

            public void Move() {
                if (_Delay == 0) {
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

        public StarField() {
            for (int y=0; y < TUI.BottomEdge; y++) { starfield.Add(new Star(y));}
        }

        #endregion

        #region Animate

        public void Animate() {

                if (starfield.FindAll(x => x.OffScreen).Count > 0) {
                    starfield.Remove(starfield.Find(x => x.OffScreen));
                }
                while (starfield.Count < TUI.BottomEdge) {
                    starfield.Add(new Star());
                }
                foreach (var s in starfield) {
                    s.Move();
                }

        }

        #endregion

    } */

    #endregion

    class Program
    {
        static void Main(string[] args)
        {

            Console.Clear();
            Console.CursorVisible = false;
            



            const int defaultConsoleWindowWidth = 50;
            const int defaultConsoleWindowHeight = 50;

                OperatingSystem os = Environment.OSVersion;
                PlatformID     pid = os.Platform;

                if (pid != PlatformID.Unix && pid != (PlatformID)128) {
                    System.Console.WindowHeight = defaultConsoleWindowHeight;
                    System.Console.WindowWidth = defaultConsoleWindowWidth;
                }else{
                    //assume *NIX system
                    try {
                        var p = new System.Diagnostics.Process();
                        p.StartInfo = new System.Diagnostics.ProcessStartInfo(@"stty cols " + defaultConsoleWindowWidth + " rows " + defaultConsoleWindowHeight, "-n")
                        {
                            UseShellExecute = false
                        };

                        p.Start();
                        p.WaitForExit();
                    }
                    catch (Exception e) { /*...*/}


                }





          
        

            int MaxEnemies = 100;

            var enemies = new List<Enemy>();
            var stars = new StarField();

            // Main loop

            do {
                
                stars.Animate();
                
                // Zap one out-of-bounds enemy

                if (enemies.FindAll(x => x.FellOffTheScreen).Count > 0) {
                    enemies.Remove(enemies.Find(x => x.FellOffTheScreen));
                }
 
                foreach (var e in enemies) {
                    e.Move();
                }


                // Spawn enemies

                while (MaxEnemies > enemies.Count) {
                    enemies.Add(new Enemy());
                }

                // Move enemies
                foreach (var e in enemies) {
                    e.Move();
        
                }


                // Scrolling bug indicator (for debugging only)
                Console.SetCursorPosition(TUI.LeftEdge+10,TUI.BottomEdge); Console.Write("-");
            
                Thread.Sleep(100);
            


            } while (!Console.KeyAvailable);




             Console.SetCursorPosition(0,TUI.BottomEdge);
             Console.CursorVisible = true;
        }
    }
}
