using BenchmarkDotNet.Running;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
          var switcher = new BenchmarkSwitcher(new[]
          {
            typeof(StopwatchBench),
            typeof(StopwatchBench2)
          });
          switcher.Run();
        }
    }
}
