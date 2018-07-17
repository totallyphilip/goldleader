using System;
using System.Threading;
using System.Collections.Generic;

namespace TieInvaders
{
    class Program

    {

        static void Play()
        {

            Console.Clear();

            int MaxEnemies = 10;

            var enemies = new List<Enemy>();
            var stars = new StarField();

            // Main loop

            do
            {

                stars.Animate();

                // Zap one out-of-bounds enemy

                if (enemies.FindAll(x => x.FellOffTheScreen).Count > 0)
                {
                    enemies.Remove(enemies.Find(x => x.FellOffTheScreen));
                }

                foreach (var e in enemies)
                {
                    e.Move();
                }


                // Spawn enemies

                while (MaxEnemies > enemies.Count)
                {
                    enemies.Add(new Enemy());
                }

                // Move enemies
                foreach (var e in enemies)
                {
                    e.Move();

                }


                // Scrolling bug indicator (for debugging only)
                Console.SetCursorPosition(Textify.LeftEdge + 10, Textify.BottomEdge); Console.Write("-");

                Thread.Sleep(100);



            } while (!Console.KeyAvailable);

            Console.SetCursorPosition(0, Textify.BottomEdge);
            Console.CursorVisible = true;
        }

        static void Main(string[] args)
        {
            if (Textify.SetWindowSizeSafely(50, 35))
            {
                  Play();
            }
            else {
                Textify.WaitPrompt("something went horribly wrong");
            }
        }
    }
}
