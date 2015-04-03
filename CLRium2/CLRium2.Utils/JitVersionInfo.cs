using System;

namespace CLRium2.Utils
{
    public class JitVersionInfo
    {
        public static JitVersion GetJitVersion()
        {
            return new JitVersionInfo().GetJitVersionInternal();
        }

        private JitVersion GetJitVersionInternal()
        {
            if (IsMono())
                return JitVersion.Mono;
            if (IsMsX86())
                return JitVersion.MsX86;
            if (IsMsX64())
                return JitVersion.MsX64;
            return JitVersion.RyuJit;
        }

        private int bar;

        private bool IsMsX64(int step = 1)
        {
            var value = 0;
            for (int i = 0; i < step; i++)
            {
                bar = i + 10;
                for (int j = 0; j < 2 * step; j += step)
                    value = j + 10;
            }
            return value == 20 + step;
        }

        private static bool IsMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        private static bool IsMsX86()
        {
            return !IsMono() && IntPtr.Size == 4;
        }
    }
}