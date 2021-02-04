using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace KeybaordAudio
{
    class Program
    {
        const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess,
          int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        static void Main(string[] args)
        {
            KeyboardControl kb = new KeyboardControl();
            Process process = Process.GetProcessesByName("hl2")[0];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);

            int bytesRead = 0;
            byte[] buffer = new byte[1]; //'Hello World!' takes 12*2 bytes because of Unicode 
            int offset = 0xDEC4;
            int found = 0;
            int finaloffset = 0;
            for (int i = 0x10000; offset <= Int32.MaxValue ; i = 0x10000)
            {
                offset += i;
                ReadProcessMemory((int)processHandle, offset, buffer, buffer.Length, ref bytesRead);
                if (buffer[0] == 175)
                {
                    Console.WriteLine(offset.ToString("X") + " meets criteria!");
                    found++;
                    if(finaloffset == 0)
                        finaloffset = offset;
                }
                if (offset > 0xFFFDEC4)
                    break;
            }
            double foundcertain = 1d / found;
            Console.WriteLine("{0} percent certainty.", foundcertain * 100);
            while (true)
            {
                ReadProcessMemory((int)processHandle, finaloffset, buffer, buffer.Length, ref bytesRead);
                kb.LightLoop(buffer[0]);
            }
        }
    }
}

