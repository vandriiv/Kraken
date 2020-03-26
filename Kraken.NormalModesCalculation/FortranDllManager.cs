using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Kraken.NormalModesCalculation
{
    class FortranDllManager
    {
        [DllImport("UnmanagedDlls/TWERSK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TWERSK(ref char OPT, ref double OMEGA, ref double BUMDEN, ref double XI, ref double ETA,
                                ref double KX, ref double RHO0, ref double C0, ref double C1, ref double C2);       
    }
}
