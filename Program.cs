using System;

namespace MyProgram
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("1 - star wars");
            Console.WriteLine("2 - zombies (under development)");
            
            ConsoleKeyInfo k = Console.ReadKey(true);
            if (k.Key == ConsoleKey.D1) {
                GoldLeader game = new GoldLeader();
                game.TryPlay();
            }
            else if (k.Key == ConsoleKey.D2)
            {

                Zombie z = new Zombie();
                z.TryPlay();
            }

        }
    }
}
