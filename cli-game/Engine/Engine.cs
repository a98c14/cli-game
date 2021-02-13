using Game.Vector;
using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Game.Engine
{
    public class GameEngine
    { 
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
        private struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Coord
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
        private struct CharUnion
        {
            [FieldOffset(0)] public char UnicodeChar;
            [FieldOffset(0)] public byte AsciiChar;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct CharInfo
        {
            [FieldOffset(0)] public CharUnion Char;
            [FieldOffset(2)] public short Attributes;
        }

        private readonly SafeFileHandle m_FileHandle;
        private readonly CharInfo[] m_InitialBuffer;
        private readonly CharInfo[] m_DrawBuffer;
        private readonly SmallRect m_SmallRect;

        private readonly int m_Width;
        private readonly int m_Height;

        public GameEngine(short width, short height)
        {
            m_Width = width;
            m_Height = height;

            // Create the inital state buffer
            CreateBuffer(width, height, out m_FileHandle, out m_InitialBuffer, out m_SmallRect);

            // Copy the contents to draw buffer
            m_DrawBuffer = new CharInfo[m_InitialBuffer.Length];
            Array.Copy(m_InitialBuffer, m_DrawBuffer, m_DrawBuffer.Length);
        }

        public void ClearBuffer()
        {
            Array.Copy(m_InitialBuffer, m_DrawBuffer, m_InitialBuffer.Length);
        }

        public void Draw()
        {
            DrawBuffer(m_FileHandle, m_DrawBuffer, m_SmallRect);
        }

        public void Set(Vector2 pos, char c, CharAttribute attr = CharAttribute.FOREGROUND_RED)
        {
            Set(pos.x, pos.y, c, attr);
        }

        public void Set(int x, int y, char c, CharAttribute attr = CharAttribute.FOREGROUND_RED)
        {
            var index = x + y * m_Width;
            var ci = m_DrawBuffer[index];
            ci.Attributes = (short)attr;
            ci.Char.UnicodeChar = c;
            m_DrawBuffer[index] = ci;
        }

        public void Set(int x, int y, byte b, CharAttribute attr = CharAttribute.FOREGROUND_RED)
        {
            var index = x + y * m_Width;
            var ci = m_DrawBuffer[index];
            ci.Attributes = (short)attr;
            ci.Char.AsciiChar = b;
            m_DrawBuffer[index] = ci;
        }

        private static void DrawBuffer(SafeFileHandle handle, CharInfo[] buffer, SmallRect rect)
        {
            WriteConsoleOutput(handle, buffer,
                new Coord() { X = rect.Right, Y = rect.Bottom },
                new Coord() { X = rect.Left, Y = rect.Top },
                ref rect);
        }

        private static void CreateBuffer(short width, short height, out SafeFileHandle handle, out CharInfo[] buffer, out SmallRect rect)
        {
            handle = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            buffer = new CharInfo[width * height];
            rect = new SmallRect() { Left = 0, Top = 0, Right = width, Bottom = height };
            var rand = new Random();            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var b = rand.Next(0, 100) > 90;
                    buffer[x + width * y] = new CharInfo
                    {
                        Attributes = (short)CharAttribute.FOREGROUND_GREEN,
                        Char = new CharUnion
                        {
                            UnicodeChar = b ? 'I' : '_',
                        }
                    };
                }
            }
        }
    }
}
