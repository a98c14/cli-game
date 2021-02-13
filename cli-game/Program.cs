using System;
using System.Windows.Input;
using Game.Engine;

namespace Game
{
    class Program
    {
        const string m_GameVersion = "0.0.1";

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine($"Environment Version: .NET {Environment.Version}");
            Console.WriteLine($"Game Version: {m_GameVersion}");
            Console.WriteLine($"Staring Game...");
            var engine = new GameEngine(80, 30);
            while (true)
            {
                engine.Draw();
                var state = engine.GetKeyStates(VirtualKeyStates.VK_DOWN);
                engine.Set(20, 20, state.ToString().ToCharArray()[0], CharAttribute.BACKGROUND_RED | CharAttribute.BACKGROUND_BLUE);
            }
            Console.ReadKey();
        }
    }
}
