using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace Benchmarks
{
  [Config(typeof(Config))]
  public class StopwatchBench2
  {
    [Benchmark]
    public long StopwatchLatency()
    {
      return Stopwatch.GetTimestamp();
    }

    [Benchmark]
    public long StopwatchGranularity()
    {
      long lastTimestamp = Stopwatch.GetTimestamp();
      while (Stopwatch.GetTimestamp() == lastTimestamp)
      {
      }
      return lastTimestamp;
    }

    private class Config : ManualConfig
    {
      public Config()
      {
        Add(Job.Clr.WithLaunchCount(1));
        Add(Job.Mono.WithLaunchCount(1));
      }
    }
  }
}