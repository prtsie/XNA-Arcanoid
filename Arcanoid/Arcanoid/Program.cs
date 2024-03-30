namespace Arcanoid
{
    static internal class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            using ArcanoidGame game = new();
            game.Run();
        }
    }
}
