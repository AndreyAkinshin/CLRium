using System;
using System.Runtime.CompilerServices;
using CLRium2.Utils;

namespace CLRium2.Example4_JitX64Bug
{
    class Program
    {
        static void Main()
        {
            new Program().Run();
        }

        private void Run()
        {
            Console.WriteLine("JIT: " + JitVersionInfo.GetJitVersion());
            Console.WriteLine("Optimization:");
            Optimization(1);
            Console.WriteLine("NoOptimization:");
            NoOptimization(1);
        }

        int bar;

        public void Optimization(int step)
        {
            for (int i = 0; i < step; i++)
            {
                bar = i + 10;
                for (int j = 0; j < 2 * step; j += step)
                    Console.WriteLine(j + 10);
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void NoOptimization(int step)
        {
            for (int i = 0; i < step; i++)
            {
                bar = i + 10;
                for (int j = 0; j < 2 * step; j += step)
                    Console.WriteLine(j + 10);
            }
        }
    }
}

// См. также:
// http://aakinshin.net/ru/blog/dotnet/subexpression-elimination-bug-in-jit-x64/