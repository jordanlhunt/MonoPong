internal class Program
{
    private static void Main(string[] args)
    {
        using var game = new MonoPong.Game1();
        game.Run();
    }
}