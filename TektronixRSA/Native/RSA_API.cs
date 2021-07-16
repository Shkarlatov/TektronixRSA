using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using static System.Runtime.InteropServices.CallingConvention;

namespace Tektronix.TekRSA.Native
{
    [System.Security.SuppressUnmanagedCodeSecurity]
    static class RSA_API
    {
        private const string DLL_NAME = "RSA_API.dll";

        static RSA_API()
        {
            var arch = IntPtr.Size == 8 ? "x64" : "x86";
            var currentDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, arch);
            AddEnvironmentPaths(new []{ currentDir });
        }

        private static void AddEnvironmentPaths(IEnumerable<string> paths)
        {
            var path = new[] { Environment.GetEnvironmentVariable("PATH") ?? string.Empty };
            string newPath = string.Join(Path.PathSeparator.ToString(), path.Concat(paths));
            Environment.SetEnvironmentVariable("PATH", newPath);
        }

        ///////////////////////////////////////////////////////////
        // Device Connection and Info
        ///////////////////////////////////////////////////////////
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus DEVICE_Connect(int deviceID);
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus DEVICE_Reset(int deviceID);
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus DEVICE_Disconnect();

        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus DEVICE_GetOverTemperatureStatus(out bool overTemperature);

        ///////////////////////////////////////////////////////////
        // Device Operation (global)
        ///////////////////////////////////////////////////////////
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus DEVICE_GetEnable(out bool enable);
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus DEVICE_Run();
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus DEVICE_Stop();
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus DEVICE_PrepareForRun();
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus DEVICE_StartFrameTransfer();

        ///////////////////////////////////////////////////////////
        // Device Configuration (global)
        ///////////////////////////////////////////////////////////
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus CONFIG_Preset();
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus CONFIG_SetReferenceLevel(double refLevel);
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus CONFIG_GetReferenceLevel(out double refLevel);
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus CONFIG_GetMaxCenterFreq(out double maxCF);
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus CONFIG_GetMinCenterFreq(out double minCF);
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus CONFIG_SetCenterFreq(double cf);
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus CONFIG_GetCenterFreq(out double cf);

        ///////////////////////////////////////////////////////////
        // Device Alignment
        ///////////////////////////////////////////////////////////
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus ALIGN_GetWarmupStatus(out bool warmedUp);
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus ALIGN_GetAlignmentNeeded(out bool needed);
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus ALIGN_RunAlignment();

        ///////////////////////////////////////////////////////////
        // Spectrum Trace acquisition
        ///////////////////////////////////////////////////////////

        //  Enable/disable Spectrum measurement
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus SPECTRUM_SetEnable(bool enable);
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus SPECTRUM_GetEnable(out bool enable);

        //  Set spectrum settings to default values
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus SPECTRUM_SetDefault();

        //  Set/get spectrum settings
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus SPECTRUM_SetSettings(SpectrumSettings settings);
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus SPECTRUM_GetSettings(out SpectrumSettings settings);

        //  Set/get spectrum trace settings
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus SPECTRUM_SetTraceType(SpectrumTraces trace,bool enable, SpectrumDetectors detector);
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus SPECTRUM_GetTraceType(SpectrumTraces trace, out bool enable,out SpectrumDetectors detector);

        //  Get spectrum setting limits
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus SPECTRUM_GetLimits(out SpectrumLimits limits);

        //  Start Trace acquisition
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus SPECTRUM_AcquireTrace();
        //  Wait for spectrum trace ready
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus SPECTRUM_WaitForTraceReady(int timeoutMsec, out bool ready);

        //  Get spectrum trace data
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus SPECTRUM_GetTrace(SpectrumTraces trace, int maxTracePoints, [Out] float[] traceData, out int outTracePoints);

        //  Get spectrum trace result information
        [DllImport(
            dllName: DLL_NAME,
            CallingConvention = StdCall,
            ExactSpelling = true)]
        public static extern ReturnStatus SPECTRUM_GetTraceInfo(out SpectrumTraceInfo traceInfo);

    }
}
