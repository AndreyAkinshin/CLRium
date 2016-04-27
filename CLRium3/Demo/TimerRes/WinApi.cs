using System.Runtime.InteropServices;

namespace TimerRes
{
  public struct ResolutionInfo
  {
    public uint Min;
    public uint Max;
    public uint Current;
  }

  public class WinApi
  {
    [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]
    public static extern uint TimeBeginPeriod(uint uMilliseconds);

    [DllImport("winmm.dll", EntryPoint = "timeEndPeriod", SetLastError = true)]
    public static extern uint TimeEndPeriod(uint uMilliseconds);

    [DllImport("ntdll.dll", SetLastError = true)]
    private static extern uint NtQueryTimerResolution(out uint min, out uint max, out uint current);

    [DllImport("ntdll.dll", SetLastError = true)]
    private static extern uint NtSetTimerResolution(uint desiredResolution, bool setResolution,
      ref uint currentResolution);

    [DllImport("kernel32.dll")]
    private static extern bool QueryPerformanceCounter(out long value);

    [DllImport("kernel32.dll")]
    private static extern bool QueryPerformanceFrequency(out long value);

    public static ResolutionInfo QueryTimerResolution()
    {
      var info = new ResolutionInfo();
      NtQueryTimerResolution(out info.Min, out info.Max, out info.Current);
      return info;
    }

    public static ulong SetTimerResolution(uint ticks)
    {
      uint currentRes = 0;
      NtSetTimerResolution(ticks, true, ref currentRes);
      return currentRes;
    }

    public static long Qpc()
    {
      long value;
      QueryPerformanceCounter(out value);
      return value;
    }

    public static long Qpf()
    {
      long value;
      QueryPerformanceFrequency(out value);
      return value;
    }
  }
}