using System;

namespace Platform_Creator_CS.Sources {
    public static class Program {
        [STAThread]
        private static void Main() {
            using (var game = new PCGame()) {
                game.Run();
            }
        }
    }
}