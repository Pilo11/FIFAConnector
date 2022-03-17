using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace FIFAConnectorClient;
internal static class MemoryManager
{
    private const int ProcessAll = 0x001F0FFF;

    [Flags]
    private enum AllocationType
    {
        Commit = 0x1000,
        Reserve = 0x2000,
        Decommit = 0x4000,
        Release = 0x8000,
        Reset = 0x80000,
        Physical = 0x400000,
        TopDown = 0x100000,
        WriteWatch = 0x200000,
        LargePages = 0x20000000
    }

    [Flags]
    private enum MemoryProtection
    {
        Execute = 0x10,
        ExecuteRead = 0x20,
        ExecuteReadWrite = 0x40,
        ExecuteWriteCopy = 0x80,
        NoAccess = 0x01,
        ReadOnly = 0x02,
        ReadWrite = 0x04,
        WriteCopy = 0x08,
        GuardModifierflag = 0x100,
        NoCacheModifierflag = 0x200,
        WriteCombineModifierflag = 0x400
    }

    [DllImport("kernel32.dll")]
    static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

    [DllImport("kernel32.dll")]
    static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int nSize, int lpNumberOfBytesWritten);

    internal static IntPtr AllocSpace(IntPtr handle, uint byteArraySize)
    {
        return VirtualAllocEx(handle, IntPtr.Zero, byteArraySize, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ExecuteReadWrite);
    }

    internal static byte[] GetByteArrayFromMemory(IntPtr handle, int address, int length)
    {
        int bytesRead = 0;
        byte[] buffer = new byte[length];
        ReadProcessMemory(handle.ToInt32(), address, buffer, buffer.Length, ref bytesRead);
        return buffer;
    }

    internal static void WriteByteArrayFromMemory(IntPtr handle, int address, byte[] bytes)
    {
        WriteProcessMemory(handle, address, bytes, bytes.Length, 0);
    }

    internal static IntPtr GetProcHandle()
    {
        return OpenProcess(ProcessAll, true, Process.GetProcessesByName("fifa").FirstOrDefault()?.Id ?? 0);
    }

}