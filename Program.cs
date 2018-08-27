namespace MyProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            TheGame game = new TheGame();
            int score = 0;
            game.TryPlay(score);
            System.Environment.Exit(0);
        }
    }
}
