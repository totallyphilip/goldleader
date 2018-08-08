namespace MyProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            AsciiWars game = new AsciiWars();
            int score = 0;
            game.TryPlay(score);
            System.Environment.Exit(0);
        }
    }
}
