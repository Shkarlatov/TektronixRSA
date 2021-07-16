using System;
using System.Collections.Generic;
using System.Text;

using Tektronix.TekRSA.Native;

namespace Tektronix.TekRSA
{
    public class Spectrum : TekBase
    {
        internal Spectrum()
        {
            SetDefault();
            spectrumLimits = GetLimits();
            SettingsGetter();
        }

        internal SpectrumLimits spectrumLimits;
        internal SpectrumSettings spectrumSettings;

        public SpectrumTrace[] Traces { get; } = new[]
        {
            new SpectrumTrace(SpectrumTraces.Trace1),
            new SpectrumTrace(SpectrumTraces.Trace2),
            new SpectrumTrace(SpectrumTraces.Trace3)
        };



        public double Span
        {
            get => spectrumSettings.span;
            set
            {
                spectrumSettings.span = value;
                SettingsSetter();
            }
        }
        public double RBW
        {
            get => spectrumSettings.rbw;
            set
            {
                spectrumSettings.rbw = value;
                SettingsSetter();
            }
        }

        public bool EnableVBW
        {
            get => spectrumSettings.enableVBW;
            set
            {
                spectrumSettings.enableVBW = value;
                SettingsSetter();
            }
        }
        public double VBW
        {
            get => spectrumSettings.vbw;
            set
            {
                spectrumSettings.vbw = value;
                SettingsSetter();
            }
        }
        public int TraceLength
        {
            get => spectrumSettings.traceLength;
            set
            {
                if (value % 2 == 0) // must be odd
                    value++;
                    //throw new ArgumentException("TraceLength must be odd number");
                spectrumSettings.traceLength = value;
                SettingsSetter();
            }
        }

        public SpectrumWindows SpectrumWindows
        {
            get => spectrumSettings.window;
            set
            {
                spectrumSettings.window = value;
                SettingsSetter();
            }
        }

        public SpectrumVerticalUnits VerticalUnits
        {
            get => spectrumSettings.verticalUnit;
            set
            {
                spectrumSettings.verticalUnit = value;
                SettingsSetter();
            }
        }


        void SettingsGetter()
        {
            spectrumSettings = GetSettings();
        }
        void SettingsSetter()
        {
            spectrumSettings.rbw =
                spectrumSettings.rbw
                .Limit(
                    spectrumLimits.minRBW,
                    spectrumLimits.maxRBW);

            spectrumSettings.span =
                spectrumSettings.span
                .Limit(
                    spectrumLimits.minSpan,
                    spectrumLimits.maxSpan);

            spectrumSettings.vbw =
                spectrumSettings.vbw
                .Limit(
                    spectrumLimits.minVBW,
                    spectrumLimits.maxVBW);

            spectrumSettings.traceLength =
                spectrumSettings.traceLength
                .Limit(
                    spectrumLimits.minTraceLength,
                    spectrumLimits.maxTraceLength);

            SetSettings(spectrumSettings);
        }


        /// <summary>
        /// Enable status.
        /// </summary>
        public bool Enable
        {
            get => GetEnable();
            set => SetEnable(value);
        }

        void SetEnable(bool enable)
        {
            var rs = RSA_API.SPECTRUM_SetEnable(enable);
            rs.CheckError(this);
        }

        bool GetEnable()
        {
            var rs = RSA_API.SPECTRUM_GetEnable(out bool enable);
            rs.CheckError(this);
            return enable;
        }

        /// <summary>
        /// Sets the spectrum settings to default settings.
        /// Span: 40 MHz
        /// RBW: 300 kHz
        /// Enable VBW: false
        /// VBW: 300 kHz
        /// Trace Length: 801
        /// Window: Kaiser
        /// Vertical Unit: dBm
        /// Trace1: Enable, +Peak
        /// Trace2: Disable, -Peak
        /// Trace3: Disable, Average
        /// </summary>
        public void SetDefault()
        {
            var rs = RSA_API.SPECTRUM_SetDefault();
            rs.CheckError(this);
        }

        SpectrumLimits GetLimits()
        {
            var limits = new SpectrumLimits();
            var rs = RSA_API.SPECTRUM_GetLimits(out limits);
            rs.CheckError(this);
            return limits;
        }

        /// <summary>
        /// Initiates a spectrum trace acquisition.
        /// Executing this function initiates a spectrum trace acquisition. Before calling this
        /// function, all acquisition parameters must be set to valid states.These include
        /// Center Frequency, Reference Level, any desired Trigger conditions, and the
        /// SPECTRUM configuration settings.
        /// </summary>
        public void AcquireTrace()
        {
            var rs = RSA_API.SPECTRUM_AcquireTrace();
            rs.CheckError(this);
        }

        public bool WaitForTraceReady(int timeoutMsec = 100)
        {
            bool ready = false;
            var rs = RSA_API.SPECTRUM_WaitForTraceReady(timeoutMsec, out ready);
            rs.CheckError(this);
            return ready;
        }


        public int GetTrace(SpectrumTraces trace, ref float[] traceData)
        {
            var rs = RSA_API.SPECTRUM_GetTrace(trace, traceData.Length, traceData, out int count);
            rs.CheckError(this);
            return count;
        }

        void SetSettings(SpectrumSettings settings)
        {
            var rs = RSA_API.SPECTRUM_SetSettings(settings);
            rs.CheckError(this);
        }
        SpectrumSettings GetSettings()
        {
            var rs = RSA_API.SPECTRUM_GetSettings(out SpectrumSettings settings);
            rs.CheckError(this);
            return settings;
        }


        //void SetTraceType(SpectrumTraces trace, bool enable, SpectrumDetectors detector)
        //{
        //    var rs = RSA_API.SPECTRUM_SetTraceType(trace, enable, detector);
        //    rs.CheckError(this);
        //}
        //void GetTraceType(SpectrumTraces trace, bool enable, SpectrumDetectors detector)
        //{
        //    var rs = RSA_API.SPECTRUM_GetTraceType(trace,  out enable, out detector);
        //    rs.CheckError(this);
        //}
    }

}
