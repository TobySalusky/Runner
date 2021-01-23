using System;

namespace Runner
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Runner())
                game.Run();
        }
    }
}
