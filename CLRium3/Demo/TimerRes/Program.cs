using System;
using System.Diagnostics;

namespace TimerRes
{
  class Program
  {
    static void Main(string[] args)
    {
      var resolutioInfo = WinApi.QueryTimerResolution();
      Console.WriteLine($"Min = {resolutioInfo.Min}");
      Console.WriteLine($"Max = {resolutioInfo.Max}");
      Console.WriteLine($"Current = {resolutioInfo.Current}");
    }
  }
}