namespace MyProgram
{
    class Program
    {
        static void Main(string[] args)
        {

            /*             PowerUp pow = new PowerUp(PowerUp.ePowerType.SaySomething, new AsciiEngine.Screen.Coordinate(10, 10), new AsciiEngine.Screen.Trajectory());
                        pow.Animate();
                        System.Console.WriteLine();
                        pow.SayStuff();
                        System.Console.ReadKey(); */

            TieFighterGame game = new TieFighterGame();
            game.TryPlay();
            System.Environment.Exit(0);

        }
    }
}
