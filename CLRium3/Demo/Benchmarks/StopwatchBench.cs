using System.Diagnostics;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
  public class StopwatchBench
  {
    [Benchmark]
    public long StopwatchLatency()
    {
      return Stopwatch.GetTimestamp();
    }
  }
}