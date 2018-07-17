using System;
using System.Collections.Generic;
using System.Threading;
public class TieFight
{

    public void TryPlay()
    {

        int oldwidth = Console.WindowWidth;
        int oldheight = Console.WindowHeight;

        if (Textify.SetWindowSizeSafely(50, 35))
        {
            Console.CursorVisible = false;
            this.Play();
            Console.CursorVisible = true;
        }
        else
        {
            Textify.WaitPrompt("something went horribly wrong");
        }

        Textify.SetWindowSizeSafely(oldwidth, oldheight, false);

    }

    public void Play()
    {

        Console.Clear();

        int MaxEnemies = 10;

        var enemies = new List<Enemy>();
        StarField stars = new StarField();
        EnemyFleet fleet = new EnemyFleet();

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

    }
}