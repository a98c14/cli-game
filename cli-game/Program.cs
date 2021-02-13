using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace cli_game
{
    class Program
    {
        const string m_GameVersion = "0.0.1";

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutput(
            SafeFileHandle hConsoleOutput,
            CharInfo[] lpBuffer,
            Coord dwBufferSize,
            Coord dwBufferCoord,
            ref SmallRect lpWriteRegion);

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct CharUnion
        {
            [FieldOffset(0)] public char UnicodeChar;
            [FieldOffset(0)] public byte AsciiChar;
        }


        [StructLayout(LayoutKind.Explicit)]
        public struct CharInfo
        {
            [FieldOffset(0)] public CharUnion Char;
            [FieldOffset(2)] public short Attributes;
        }

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine($"Environment Version: .NET {Environment.Version}");
            Console.WriteLine($"Game Version: {m_GameVersion}");
            Console.WriteLine($"Staring Game...");

            
            SetupBoard(100, 30, out var handle, out var buffer, out var rect);
            while(true)
            {
                DrawBoard(handle, buffer, rect);
            }
            Console.ReadKey();
        }

        static void DrawBoard(SafeFileHandle handle, CharInfo[] buffer, SmallRect rect)
        {
            WriteConsoleOutput(handle, buffer,
                new Coord() { X = rect.Right, Y = rect.Bottom },
                new Coord() { X = rect.Left, Y = rect.Top },
                ref rect);
        }

        static void SetupBoard(short width, short height, out SafeFileHandle handle, out CharInfo[] buffer, out SmallRect rect)
        {
            handle = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            buffer = new CharInfo[width * height];
            rect   = new SmallRect() { Left = 0, Top = 0, Right = width, Bottom = height };

            for (int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    buffer[x + width * y] = new CharInfo
                    {
                        Attributes = 3,
                        Char = new CharUnion
                        {
                            AsciiChar = 40,
                        }
                    };
                }
            }
        }
    }
}
