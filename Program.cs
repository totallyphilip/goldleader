namespace MyProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            UnicodeWars game = new UnicodeWars();
            int score = 0;
            game.TryPlay(score);
            System.Environment.Exit(0);
        }
    }
}
