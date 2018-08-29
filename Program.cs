namespace MyProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            TheGame game = new TheGame();
             game.TryPlay();
            System.Environment.Exit(0);
        }
    }
}
