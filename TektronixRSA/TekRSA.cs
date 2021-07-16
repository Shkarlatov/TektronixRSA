using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Tektronix.TekRSA.Native;

namespace Tektronix.TekRSA
{
    public class TekRSA :TekBase, IDisposable
    {
        private bool disposed = false;
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Disconnect();
                }
            }
            this.disposed = true;
        }

        void  SetupError()
        {
            Configure.Error += (o, e) => OnError(e.Message, e.Source);
            Alignment.Error += (o, e) => OnError(e.Message, e.Source);
            Spectrum.Error += (o, e) => OnError(e.Message, e.Source);
        }
        void SetupProperyChanged()
        {
            Configure.PropertyChanged += (o, e) => OnPropertyChanged(e.PropertyName);
            Alignment.PropertyChanged += (o, e) => OnPropertyChanged(e.PropertyName);
            Spectrum.PropertyChanged += (o, e) => OnPropertyChanged(e.PropertyName);
        }


        private int _deviceId;
        /// <summary>
        /// Connects to a device specified by the deviceID parameter.
        /// </summary>
        /// <param name="deviceId">Device ID found during the Search function call.
        /// "0" is default.</param>
        public void Connect(int deviceId = 0)
        {
            _deviceId = deviceId;
            var rs = RSA_API.DEVICE_Connect(deviceId);
            var ok=rs.CheckError(this);
            if(ok)
            {
                SetupError();
                SetupProperyChanged();
            }
            

        }
        /// <summary>
        /// Stops data acquisition and disconnects from the connected device.
        /// </summary>
        public void Disconnect()
        {
            RSA_API.DEVICE_Disconnect();
        }
        /// <summary>
        /// Reboots the specified device.
        /// </summary>
        public void Reset()
        {
            var rs = RSA_API.DEVICE_Reset(_deviceId);
            rs.CheckError(this);
        }


        /// <summary>
        /// Queries the run state.
        /// </summary>
        /// <returns></returns>
        bool Enable()
        {
            var rs = RSA_API.DEVICE_GetEnable(out bool run);
            rs.CheckError(this);
            return run;
        }

        /// <summary>
        /// Queries the run state.
        /// </summary>
        public bool IsActive => Enable();

        /// <summary>
        /// Starts data acquisition.
        /// </summary>
        public void Run()
        {
            var rs = RSA_API.DEVICE_Run();
            rs.CheckError(this);
        }
        /// <summary>
        /// Stops data acquisition.
        /// </summary>
        public void Stop()
        {
            var rs = RSA_API.DEVICE_Stop();
            rs.CheckError(this);
        }
        /// <summary>
        /// Performs all of the internal tasks necessary to put the system in a known stateready to stream data,
        /// but does not actually initiate data transfer.
        /// </summary>
        public void PrepareForRun()
        {
            var rs = RSA_API.DEVICE_PrepareForRun();
            rs.CheckError(this);
        }

        /// <summary>
        /// Queries for device over-temperature status.
        /// </summary>
        /// <returns>True indicates the internal device temperature is above nominal safe operating range, and may result in reduced accuracy and/or damage to the device.
        /// False indicates the device temperature is within the safe operating range.</returns>
        public bool IsOverTemperature => GetOverTemperatureStatus();
        bool GetOverTemperatureStatus()
        {
            var rs = RSA_API.DEVICE_GetOverTemperatureStatus(out bool status);
            rs.CheckError(this);
            return status;
        }


        private readonly Lazy<Configure> _lazyConfigure = new Lazy<Configure>(() => new Configure());
        private readonly Lazy<Alignment> _lazyAlignment = new Lazy<Alignment>(() => new Alignment());
        private readonly Lazy<Spectrum> _lazySpectrum = new Lazy<Spectrum>(() => new Spectrum());

        public Configure Configure => _lazyConfigure.Value;
        public Alignment Alignment => _lazyAlignment.Value;
        public Spectrum Spectrum => _lazySpectrum.Value;

        //public Configure Configure { get; } = new Configure();
        //public Alignment Alignment { get; } = new Alignment();
        //public Spectrum Spectrum { get; } = new Spectrum();
    }

}
