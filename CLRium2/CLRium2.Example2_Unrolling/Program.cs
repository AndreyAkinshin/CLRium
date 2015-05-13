using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace CLRium2.Example2_Unrolling
{
    //    Theory
    //
    //    Before loop unrolling:
    //
    //    for (int i = 0; i < 1024; i++)
    //        Foo(i);
    //
    //    After loop unrolling:
    //
    //    for (int i = 0; i < 1024; i += 4)
    //    {
    //        Foo(i);
    //        Foo(i + 1);
    //        Foo(i + 2);
    //        Foo(i + 3);
    //    }

    [Task(1, platform: BenchmarkPlatform.X86, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [Task(1, platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.LegacyJit)]
    public class UnrollBenchmarkCompetition
    {
        private const int N = 1001;
        private readonly int[] a = new int[N];

        [Benchmark]
        public int Method1()
        {
            int sum = 0;
            for (int i = 0; i < N; i++)
                sum += a[i];
            return sum;
        }

        [Benchmark]
        public int Method2()
        {
            int sum = 0;
            for (int i = 0; i < N - 1; i++)
                sum += a[i];
            return sum;
        }
    }

    class Program
    {
        static void Main()
        {
            new BenchmarkRunner().RunCompetition(new UnrollBenchmarkCompetition());
        }
    }
}

// См. также:
// http://aakinshin.net/ru/blog/dotnet/ryujit-ctp5-and-loop-unrolling/
// http://aakinshin.net/ru/blog/dotnet/unrolling-of-small-loops-in-different-jit-versions/
// https://ru.wikipedia.org/wiki/%D0%A0%D0%B0%D0%B7%D0%BC%D0%BE%D1%82%D0%BA%D0%B0_%D1%86%D0%B8%D0%BA%D0%BB%D0%B0
// http://en.wikipedia.org/wiki/Loop_unrolling
// http://stackoverflow.com/questions/2349211/when-if-ever-is-loop-unrolling-still-useful
// http://blogs.msdn.com/b/dotnet/archive/2013/09/30/ryujit-the-next-generation-jit-compiler.aspx
