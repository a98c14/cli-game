using System;
using System.Windows.Input;
using Game.Engine;
using Game.Vector;

namespace Game
{
    class Program
    {
        const string m_GameVersion = "0.0.1";
        const char m_Player = '*';

        static char[] m_PlayerAnimation = new char[]
        {
            '*',
            '-',
            '5',
            '6',
        };

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine($"Environment Version: .NET {Environment.Version}");
            Console.WriteLine($"Game Version: {m_GameVersion}");
            Console.WriteLine($"Staring Game...");
            var engine = new GameEngine(80, 30);
            var playerPos = new Vector2(0, 4);

            var frameCount = 0;
            while (true)
            {
                var player = m_PlayerAnimation[frameCount % m_PlayerAnimation.Length];
                engine.Set(playerPos, player, CharAttribute.BACKGROUND_RED | CharAttribute.BACKGROUND_BLUE);

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.UpArrow)    playerPos.y -= 1;
                    if (key.Key == ConsoleKey.DownArrow)  playerPos.y += 1;
                    if (key.Key == ConsoleKey.RightArrow) playerPos.x += 1;
                    if (key.Key == ConsoleKey.LeftArrow)  playerPos.x -= 1;
                }

                engine.Draw();
                engine.ClearBuffer();
                frameCount++;
            }
            Console.ReadKey();
        }
    }
}
