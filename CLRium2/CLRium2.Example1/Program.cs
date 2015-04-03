using System;
using System.Runtime.CompilerServices;
using CLRium2.Utils;

namespace CLRium2.Example1
{
    class Program
    {
        static void Main()
        {            
            Console.WriteLine("JIT: " + JitVersionInfo.GetJitVersion());
            Console.WriteLine(GetLength(0, 0, 3, 4));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static double GetLength(int x1, int y1, int x2, int y2)
        {
            int dx = x1 - x2;
            int dy = y1 - y2;
            int dxSquare = dx * dx;
            int dySquare = dy * dy;
            int lengthSquare = dxSquare + dySquare;
            double length = Math.Sqrt(lengthSquare);
            return length;
        }

        // IL
        // .method private hidebysig static 
        //     float64 GetLength (
        //         int32 x1,
        //         int32 y1,
        //         int32 x2,
        //         int32 y2
        //     ) cil managed noinlining 
        // {
        //     // Method begins at RVA 0x207c
        //     // Code size 22 (0x16)
        //     .maxstack 3
        //     .locals init (
        //         [0] int32 dy,
        //         [1] int32 dySquare
        //     )
        // 
        //     IL_0000: ldarg.0
        //     IL_0001: ldarg.2
        //     IL_0002: sub
        //     IL_0003: ldarg.1
        //     IL_0004: ldarg.3
        //     IL_0005: sub
        //     IL_0006: stloc.0
        //     IL_0007: dup
        //     IL_0008: mul
        //     IL_0009: ldloc.0
        //     IL_000a: dup
        //     IL_000b: mul
        //     IL_000c: stloc.1
        //     IL_000d: ldloc.1
        //     IL_000e: add
        //     IL_000f: conv.r8
        //     IL_0010: call float64 [mscorlib]System.Math::Sqrt(float64)
        //     IL_0015: ret
        // } // end of method Program::GetLength

        // MSJitX86
        //  mov         ebp,esp  
        //  push        eax  
        //  int         3  
        //  push        ebp  
        //  or          byte ptr [ebx+0C4D2BC2h],cl  
        //  mov         edx,ecx  
        //  mov         ecx,eax  
        //  imul        ecx,eax  
        //  mov         eax,edx  
        //  imul        eax,edx  
        //  add         eax,ecx  
        //  mov         dword ptr [ebp-4],eax  
        //  fild        dword ptr [ebp-4]  
        //  fsqrt  
        //  mov         esp,ebp  
        //  pop         ebp  
        //  ret         8  

        // MsJitX64
        //  sub         ecx,eax  
        //  sub         edx,r9d  
        //  imul        ecx,ecx  
        //  imul        edx,edx  
        //  lea         eax,[rcx+rdx]  
        //  cvtsi2sd    xmm0,eax        // (SSE2)
        //  sqrtsd      xmm0,xmm0       // (SSE2)
        //  ret  

        // RyuJIT
        //  mov eax, edx
        //  sub eax, r9d
        //  mov edx, ecx
        //  sub edx, r8d
        //  imul eax, eax
        //  imul edx, edx
        //  add eax, edx
        //  vcvtsi2sd xmm0, xmm0, eax   // (AVX)
        //  vsqrtsd xmm0, xmm0, xmm0    // (AVX)
        //  ret
    }
}
