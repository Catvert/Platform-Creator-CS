using System;

namespace Platform_Creator_CS
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new PCGame())
                game.Run();
        }
    }
}
