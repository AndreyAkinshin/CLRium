using System;
using System.Runtime.CompilerServices;
using CLRium2.Utils;

namespace CLRium2.Example3_Inlining
{
    class Program
    {
        // // Constructs a Decimal from an integer value.
        // //
        // public Decimal(int value) {
        //     //  JIT today can't inline methods that contains "starg" opcode.
        //     //  For more details, see DevDiv Bugs 81184: x86 JIT CQ: Removing the inline striction of "starg".
        //     int value_copy = value;
        //     if (value_copy >= 0) {
        //         flags = 0;
        //     }
        //     else {
        //         flags = SignMask;
        //         value_copy = -value_copy;
        //     }
        //     lo = value_copy;
        //     mid = 0;
        //     hi = 0;
        // }

        static void Main()
        {
            Console.WriteLine("JIT: " + JitVersionInfo.GetJitVersion());
            var value = 0;
            value += SimpleMethod(0x11);
            value += MethodWithStarg(0x12);
            value += MethodWithStargAggressive(0x13);
            Console.WriteLine(value);
        }

        static int SimpleMethod(int value)
        {
            return value;
        }

        static int MethodWithStarg(int value)
        {
            if (value < 0)
                value = -value;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int MethodWithStargAggressive(int value)
        {
            if (value < 0)
                value = -value;
            return value;
        }
    }
}

// См. также:
// http://aakinshin.net/ru/blog/dotnet/inlining-and-starg/
