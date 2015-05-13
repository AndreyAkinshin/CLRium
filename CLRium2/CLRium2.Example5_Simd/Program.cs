using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace CLRium2.Example5_Simd
{
    [Task(1, platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [Task(1, platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.RyuJit)]
    public class MatrixMulCompetition
    {
        private struct MyVector
        {
            public float X, Y, Z, W;

            public MyVector(float x, float y, float z, float w)
            {
                X = x;
                Y = y;
                Z = z;
                W = w;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static MyVector operator *(MyVector left, MyVector right)
            {
                return new MyVector(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);
            }
        }

        private Vector4 vector1, vector2, vector3;
        private MyVector myVector1, myVector2, myVector3;

        [Benchmark]
        public void MyMul() => myVector3 = myVector1 * myVector2;

        [Benchmark]
        public void BclMul() => vector3 = vector1 * vector2;
    }

    class Program
    {
        static void Main()
        {
            new BenchmarkRunner().RunCompetition(new MatrixMulCompetition());
        }
    }
}
