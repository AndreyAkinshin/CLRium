using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace RdtscApp
{
  class Program
  {
    [DllImport("kernel32.dll")]
    static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, uint lAllocationType, uint flProtect);

    static IntPtr Alloc(byte[] asm)
    {
      var ptr = VirtualAlloc(IntPtr.Zero, (uint) asm.Length, 0x00001000, 0x40);
      Marshal.Copy(asm, 0, ptr, asm.Length);
      return ptr;
    }

    public static byte[] RdtscAsm =
    {
      0x0F, 0x31, // rdtsc
      0xC3, // ret
    };

    delegate long RdtscDelegate();

    static void Main()
    {
      var rdtsc = Marshal.GetDelegateForFunctionPointer<RdtscDelegate>(Alloc(RdtscAsm));
      var tsc1 = rdtsc();
      var st1 = Stopwatch.GetTimestamp();
      Thread.Sleep(1000);
      var tsc2 = rdtsc();
      var st2 = Stopwatch.GetTimestamp();
      Console.WriteLine(tsc2 - tsc1);
      Console.WriteLine((tsc2 - tsc1) / 1024.0);
      Console.WriteLine(st2 - st1);
    }
  }
}