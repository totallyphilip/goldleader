using System;
using System.Threading;
using System.Collections.Generic;

namespace TieInvaders
{
    class Program
    {

        static void Main(string[] args)
        {

            TUI.FailSafeSetWindowSize(50,35);
            Console.Clear();

    
        

            int MaxEnemies = 10;

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
