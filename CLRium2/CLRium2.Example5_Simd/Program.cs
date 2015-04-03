using System;
using System.Numerics;
using BenchmarkDotNet; // Former Microsoft.Bcl.Simd
using CLRium2.Utils;

namespace CLRium2.Example5_Simd
{
    class SimdBenchmarkCompetition : BenchmarkCompetition
    {
        private const int IterationCount = 10001, VectorSize = 1 << 14;

        [BenchmarkMethod]
        public float[] AddFloatVector()
        {
            var a = new float[VectorSize];
            var b = new float[VectorSize];
            for (int iteration = 0; iteration < IterationCount; iteration++)
                for (int i = 0; i < a.Length; ++i)
                    a[i] += b[i];
            return a;
        }

        [BenchmarkMethod]
        public float[] AddFloatVectorSimd()
        {
            var a = new float[VectorSize];
            var b = new float[VectorSize];
            var simdCount = Vector<float>.Count; // Former Vector<float>.Length
            for (int iteration = 0; iteration < IterationCount; iteration++)
                for (int i = 0; i < a.Length; i += simdCount)
                {
                    var va = new Vector<float>(a, i);
                    var vb = new Vector<float>(b, i);
                    va += vb;
                    va.CopyTo(a, i);
                }
            return a;
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("JIT: " + JitVersionInfo.GetJitVersion());
            BenchmarkSettings.Instance.DetailedMode = true;
            var competition = new SimdBenchmarkCompetition();
            competition.Run();
        }
    }

    // Benchmark results
    //  MSJitX64
    //    AddFloatVector     :  104ms,  223763 ticks [Error = 17.08 %, StdDev = 4.34]
    //    AddFloatVectorSimd : 2760ms, 5917749 ticks [Error = 08.83 %, StdDev = 74.04]
    //  RyuJIT
    //    AddFloatVector     :  145ms,  312437 ticks [Error = 04.30%, StdDev = 1.60]
    //    AddFloatVectorSimd :   35ms,   77275 ticks [Error = 19.63 %, StdDev = 2.84]

    // *******************************************************************************************************
    // AddFloatVector / MsJitX64
    //             var a = new float[VectorSize];
    // 00007FFD2B302E02  sub         esp,20h  
    // 00007FFD2B302E05  mov         edx,4000h  
    // 00007FFD2B302E0A  lea         rcx,[7FFD89035EDAh]  
    // 00007FFD2B302E11  call        00007FFD8A988980  
    // 00007FFD2B302E16  mov         rbx,rax  
    //             var b = new float[VectorSize];
    // 00007FFD2B302E19  mov         edx,4000h  
    // 00007FFD2B302E1E  lea         rcx,[7FFD89035EDAh]  
    // 00007FFD2B302E25  call        00007FFD8A988980  
    // 00007FFD2B302E2A  mov         r11,rax  
    //             for (int iteration = 0; iteration < IterationCount; iteration++)
    // 00007FFD2B302E2D  xor         edx,edx  
    //                     a[i] += b[i];
    // 00007FFD2B302E2F  mov         rax,qword ptr [rbx+8]  
    //                 for (int i = 0; i < a.Length; ++i)
    // 00007FFD2B302E33  mov         eax,4000h  
    // 00007FFD2B302E38  test        eax,eax  
    // 00007FFD2B302E3A  jle         00007FFD2B302E70  
    // 00007FFD2B302E3C  xor         ecx,ecx  
    // 00007FFD2B302E3E  mov         rax,qword ptr [rbx+8]  
    // 00007FFD2B302E42  mov         rax,qword ptr [r11+8]  
    // 00007FFD2B302E46  nop         word ptr [rax+rax]  
    //                     a[i] += b[i];
    // 00007FFD2B302E50  movss       xmm0,dword ptr [rbx+rcx+10h]  
    // 00007FFD2B302E56  addss       xmm0,dword ptr [r11+rcx+10h]  
    // 00007FFD2B302E5D  movss       dword ptr [rbx+rcx+10h],xmm0  
    // 00007FFD2B302E63  add         rcx,4  
    //                 for (int i = 0; i < a.Length; ++i)
    // 00007FFD2B302E67  cmp         rcx,10000h  
    // 00007FFD2B302E6E  jl          00007FFD2B302E50  
    //             for (int iteration = 0; iteration < IterationCount; iteration++)
    // 00007FFD2B302E70  inc         edx  
    // 00007FFD2B302E72  cmp         edx,2711h  
    // 00007FFD2B302E78  jl          00007FFD2B302E33  
    //             return a;
    // 00007FFD2B302E7A  mov         rax,rbx  
    // 00007FFD2B302E7D  add         rsp,20h  
    // 00007FFD2B302E81  pop         rbx  
    // 00007FFD2B302E82  ret

    // *******************************************************************************************************
    // AddFloatVector / RyuJIT
    //             var a = new float[VectorSize];
    // 00007FFD2B2D2FB2  sub         esp,20h  
    // 00007FFD2B2D2FB5  mov         rcx,7FFD89035EDAh  
    // 00007FFD2B2D2FBF  mov         edx,4000h  
    // 00007FFD2B2D2FC4  call        00007FFD8A988980  
    // 00007FFD2B2D2FC9  mov         rsi,rax  
    //             var b = new float[VectorSize];
    // 00007FFD2B2D2FCC  mov         rcx,7FFD89035EDAh  
    // 00007FFD2B2D2FD6  mov         edx,4000h  
    // 00007FFD2B2D2FDB  call        00007FFD8A988980  
    //             for (int iteration = 0; iteration < IterationCount; iteration++)
    // 00007FFD2B2D2FE0  xor         edx,edx  
    //                 for (int i = 0; i < a.Length; ++i)
    // 00007FFD2B2D2FE2  xor         ecx,ecx  
    // 00007FFD2B2D2FE4  mov         r8d,dword ptr [rsi+8]  
    // 00007FFD2B2D2FE8  test        r8d,r8d  
    // 00007FFD2B2D2FEB  jle         00007FFD2B2D3044  
    // 00007FFD2B2D2FED  cmp         dword ptr [rax+8],r8d  
    // 00007FFD2B2D2FF1  jl          00007FFD2B2D3018  
    //                     a[i] += b[i];
    // 00007FFD2B2D2FF3  movsxd      r9,ecx  
    // 00007FFD2B2D2FF6  lea         r9,[rsi+r9*4+10h]  
    // 00007FFD2B2D2FFB  vmovss      xmm0,dword ptr [r9]  
    // 00007FFD2B2D3000  movsxd      r10,ecx  
    // 00007FFD2B2D3003  vaddss      xmm0,xmm0,dword ptr [rax+r10*4+10h]  
    // 00007FFD2B2D300A  vmovss      dword ptr [r9],xmm0  
    //                 for (int i = 0; i < a.Length; ++i)
    // 00007FFD2B2D300F  inc         ecx  
    // 00007FFD2B2D3011  cmp         r8d,ecx  
    // 00007FFD2B2D3014  jg          00007FFD2B2D2FF3  
    // 00007FFD2B2D3016  jmp         00007FFD2B2D3044  
    // 00007FFD2B2D3018  movsxd      r9,ecx  
    // 00007FFD2B2D301B  lea         r9,[rsi+r9*4+10h]  
    // 00007FFD2B2D3020  vmovss      xmm0,dword ptr [r9]  
    // 00007FFD2B2D3025  mov         r10d,dword ptr [rax+8]  
    // 00007FFD2B2D3029  cmp         ecx,r10d  
    // 00007FFD2B2D302C  jae         00007FFD2B2D3057  
    // 00007FFD2B2D302E  movsxd      r10,ecx  
    // 00007FFD2B2D3031  vaddss      xmm0,xmm0,dword ptr [rax+r10*4+10h]  
    // 00007FFD2B2D3038  vmovss      dword ptr [r9],xmm0  
    // 00007FFD2B2D303D  inc         ecx  
    // 00007FFD2B2D303F  cmp         r8d,ecx  
    // 00007FFD2B2D3042  jg          00007FFD2B2D3018  
    //             for (int iteration = 0; iteration < IterationCount; iteration++)
    // 00007FFD2B2D3044  inc         edx  
    // 00007FFD2B2D3046  cmp         edx,2711h  
    // 00007FFD2B2D304C  jl          00007FFD2B2D2FE2  
    //             return a;
    // 00007FFD2B2D304E  mov         rax,rsi  
    // 00007FFD2B2D3051  add         rsp,20h  
    // 00007FFD2B2D3055  pop         rsi  
    // 00007FFD2B2D3056  ret  
    // 00007FFD2B2D3057  call        00007FFD8AD99F00  
    // 00007FFD2B2D305C  int         3  

    // *******************************************************************************************************
    // AddFloatVectorSimd / MsJitX64
    //            var a = new float[VectorSize];
    // 00007FFD2C2D6AC1  push        rbp  
    // 00007FFD2C2D6AC2  push        rsi  
    // 00007FFD2C2D6AC3  push        rdi  
    // 00007FFD2C2D6AC4  push        r12  
    // 00007FFD2C2D6AC6  sub         rsp,70h  
    // 00007FFD2C2D6ACA  xor         eax,eax  
    // 00007FFD2C2D6ACC  mov         qword ptr [rsp+60h],rax  
    // 00007FFD2C2D6AD1  mov         qword ptr [rsp+68h],rax  
    // 00007FFD2C2D6AD6  xor         eax,eax  
    // 00007FFD2C2D6AD8  mov         qword ptr [rsp+50h],rax  
    // 00007FFD2C2D6ADD  mov         qword ptr [rsp+58h],rax  
    // 00007FFD2C2D6AE2  int         3  
    // 00007FFD2C2D6AE3  add         byte ptr [rax],al  
    // 00007FFD2C2D6AE6  add         byte ptr [rax-73h],cl  
    // 00007FFD2C2D6AE9  or          eax,5BECF3ECh  
    // 00007FFD2C2D6AEE  call        00007FFD8B958980  
    // 00007FFD2C2D6AF3  mov         rdi,rax  
    //             var b = new float[VectorSize];
    // 00007FFD2C2D6AF6  mov         edx,4000h  
    // 00007FFD2C2D6AFB  lea         rcx,[7FFD881A5EDAh]  
    // 00007FFD2C2D6B02  call        00007FFD8B958980  
    // 00007FFD2C2D6B07  mov         rbp,rax  
    // 00007FFD2C2D6B0A  mov         r12d,dword ptr [7FFD2C32FCA0h]  
    //             for (int iteration = 0; iteration < IterationCount; iteration++)
    // 00007FFD2C2D6B11  xor         ebx,ebx  
    //                 for (int i = 0; i < a.Length; i += simdCount)
    // 00007FFD2C2D6B13  mov         rax,qword ptr [rdi+8]  
    // 00007FFD2C2D6B17  nop         word ptr [rax+rax]  
    // 00007FFD2C2D6B20  mov         eax,4000h  
    // 00007FFD2C2D6B25  test        eax,eax  
    // 00007FFD2C2D6B27  jle         00007FFD2C2D6BD5  
    // 00007FFD2C2D6B2D  xor         esi,esi  
    // 00007FFD2C2D6B2F  mov         rax,qword ptr [rdi+8]  
    // 00007FFD2C2D6B33  nop         word ptr [rax+rax]  
    //                 {
    //                     var va = new Vector<float>(a, i);
    // 00007FFD2C2D6B40  mov         r8d,esi  
    // 00007FFD2C2D6B43  mov         rdx,rdi  
    // 00007FFD2C2D6B46  lea         rcx,[rsp+60h]  
    // 00007FFD2C2D6B4B  call        00007FFD2C2D6030  
    //                     var vb = new Vector<float>(b, i);
    // 00007FFD2C2D6B50  mov         r8d,esi  
    // 00007FFD2C2D6B53  mov         rdx,rbp  
    // 00007FFD2C2D6B56  lea         rcx,[rsp+50h]  
    // 00007FFD2C2D6B5B  call        00007FFD2C2D6030  
    //                     va += vb;
    // 00007FFD2C2D6B60  lea         r11,[rsp+50h]  
    // 00007FFD2C2D6B65  mov         rax,qword ptr [r11]  
    // 00007FFD2C2D6B68  mov         qword ptr [rsp+20h],rax  
    // 00007FFD2C2D6B6D  mov         rax,qword ptr [r11+8]  
    // 00007FFD2C2D6B71  mov         qword ptr [rsp+28h],rax  
    // 00007FFD2C2D6B76  lea         rcx,[rsp+60h]  
    // 00007FFD2C2D6B7B  mov         rax,qword ptr [rcx]  
    // 00007FFD2C2D6B7E  mov         qword ptr [rsp+30h],rax  
    // 00007FFD2C2D6B83  mov         rax,qword ptr [rcx+8]  
    // 00007FFD2C2D6B87  mov         qword ptr [rsp+38h],rax  
    // 00007FFD2C2D6B8C  lea         r8,[rsp+20h]  
    // 00007FFD2C2D6B91  lea         rdx,[rsp+30h]  
    // 00007FFD2C2D6B96  lea         rcx,[rsp+40h]  
    // 00007FFD2C2D6B9B  call        00007FFD2C2D60C0  
    // 00007FFD2C2D6BA0  lea         rcx,[rsp+40h]  
    // 00007FFD2C2D6BA5  mov         rax,qword ptr [rcx]  
    // 00007FFD2C2D6BA8  mov         qword ptr [rsp+60h],rax  
    // 00007FFD2C2D6BAD  mov         rax,qword ptr [rcx+8]  
    // 00007FFD2C2D6BB1  mov         qword ptr [rsp+68h],rax  
    //                     va.CopyTo(a, i);
    // 00007FFD2C2D6BB6  mov         r8d,esi  
    // 00007FFD2C2D6BB9  mov         rdx,rdi  
    // 00007FFD2C2D6BBC  lea         rcx,[rsp+60h]  
    // 00007FFD2C2D6BC1  call        00007FFD2C2D6058  
    //                 for (int i = 0; i < a.Length; i += simdCount)
    // 00007FFD2C2D6BC6  add         esi,r12d  
    // 00007FFD2C2D6BC9  cmp         esi,4000h  
    // 00007FFD2C2D6BCF  jl          00007FFD2C2D6B40  
    //             for (int iteration = 0; iteration < IterationCount; iteration++)
    // 00007FFD2C2D6BD5  inc         ebx  
    // 00007FFD2C2D6BD7  cmp         ebx,2711h  
    // 00007FFD2C2D6BDD  jl          00007FFD2C2D6B20  
    //                 }
    //             return a;
    // 00007FFD2C2D6BE3  mov         rax,rdi  
    // 00007FFD2C2D6BE6  add         rsp,70h  
    // 00007FFD2C2D6BEA  pop         r12  
    // 00007FFD2C2D6BEC  pop         rdi  
    // 00007FFD2C2D6BED  pop         rsi  
    // 00007FFD2C2D6BEE  pop         rbp  
    // 00007FFD2C2D6BEF  pop         rbx  
    // 00007FFD2C2D6BF0  ret  

    // *******************************************************************************************************
    // AddFloatVectorSimd / RyuJIT
    //             var a = new float[VectorSize];
    // 00007FFD2C2B6601  sub         rsp,20h  
    // 00007FFD2C2B6605  int         3  
    // 00007FFD2C2B6606  mov         ecx,881A5EDAh  
    // 00007FFD2C2B660B  std  
    // 00007FFD2C2B660C  jg          00007FFD2C2B660E  
    // 00007FFD2C2B660E  add         byte ptr [rdx+4000h],bh  
    // 00007FFD2C2B6614  call        00007FFD8B958980  
    // 00007FFD2C2B6619  mov         rsi,rax  
    //             var b = new float[VectorSize];
    // 00007FFD2C2B661C  mov         rcx,7FFD881A5EDAh  
    // 00007FFD2C2B6626  mov         edx,4000h  
    // 00007FFD2C2B662B  call        00007FFD8B958980  
    //             for (int iteration = 0; iteration < IterationCount; iteration++)
    // 00007FFD2C2B6630  xor         edx,edx  
    //                 for (int i = 0; i < a.Length; i += simdCount)
    // 00007FFD2C2B6632  xor         ecx,ecx  
    // 00007FFD2C2B6634  mov         r8d,dword ptr [rsi+8]  
    // 00007FFD2C2B6638  test        r8d,r8d  
    // 00007FFD2C2B663B  jle         00007FFD2C2B6674  
    //                 {
    //                     var va = new Vector<float>(a, i);
    // 00007FFD2C2B663D  lea         r9d,[rcx+7]  
    // 00007FFD2C2B6641  cmp         r9d,r8d  
    // 00007FFD2C2B6644  jae         00007FFD2C2B6687  
    // 00007FFD2C2B6646  mov         r10,rsi  
    // 00007FFD2C2B6649  vmovupd     ymm0,ymmword ptr [r10+rcx*4+10h]  
    //                     var vb = new Vector<float>(b, i);
    // 00007FFD2C2B6650  mov         r11d,dword ptr [rax+8]  
    // 00007FFD2C2B6654  cmp         r9d,r11d  
    // 00007FFD2C2B6657  jae         00007FFD2C2B6687  
    // 00007FFD2C2B6659  vmovupd     ymm1,ymmword ptr [rax+rcx*4+10h]  
    //                     va += vb;
    // 00007FFD2C2B6660  vaddps      ymm0,ymm0,ymm1  
    //                     va.CopyTo(a, i);
    // 00007FFD2C2B6665  vmovupd     ymmword ptr [r10+rcx*4+10h],ymm0  
    //                 for (int i = 0; i < a.Length; i += simdCount)
    // 00007FFD2C2B666C  add         ecx,8  
    // 00007FFD2C2B666F  cmp         r8d,ecx  
    // 00007FFD2C2B6672  jg          00007FFD2C2B663D  
    //             for (int iteration = 0; iteration < IterationCount; iteration++)
    // 00007FFD2C2B6674  inc         edx  
    // 00007FFD2C2B6676  cmp         edx,2711h  
    // 00007FFD2C2B667C  jl          00007FFD2C2B6632  
    //                 }
    //             return a;
    // 00007FFD2C2B667E  mov         rax,rsi  
    // 00007FFD2C2B6681  add         rsp,20h  
    // 00007FFD2C2B6685  pop         rsi  
    // 00007FFD2C2B6686  ret  
    // 00007FFD2C2B6687  call        00007FFD8BD69F00  
    // 00007FFD2C2B668C  int         3  
}
