using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CrashTimeUnleashed.Utils
{
    // Credits: GuidedHacking - https://www.youtube.com/watch?v=UMt1daXknes
    public static class MemoryPatcher
    {
        [DllImport("kernel32.dll")]
        static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, IntPtr lpNumberOfBytesWritten);

        const uint PAGE_EXECUTE_READWRITE = 0x40;

        public static void PatchEx(IntPtr hProcess, IntPtr dst, byte[] src)
        {
            VirtualProtectEx(hProcess, dst, (uint)src.Length, PAGE_EXECUTE_READWRITE, out uint oldProtect);
            WriteProcessMemory(hProcess, dst, src, (uint)src.Length, IntPtr.Zero);
            VirtualProtectEx(hProcess, dst, (uint)src.Length, oldProtect, out _);
        }

        public static void NopEx(IntPtr hProcess, IntPtr dst, int size)
        {
            byte[] nopArray = new byte[size];
            for (int i = 0; i < size; i++) nopArray[i] = 0x90;

            PatchEx(hProcess, dst, nopArray);
        }
    }
} 