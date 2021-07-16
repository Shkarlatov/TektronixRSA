using Tektronix.TekRSA.Native;

namespace Tektronix.TekRSA
{
    public class Configure : TekBase
    {
        internal Configure()
        {
            Preset();
        }
        /// <summary>
        /// Reference level measured in dBm.
        ///  Range: –130 dBm to 30 dBm.
        /// </summary>
        public double RefLevel
        {
            get => GetRefLevel();
            set => SetRefLevel(value);
        }

        public double CenterFreq
        {
            get => GetCenterFreq();
            set => SetCenterFreq(value);
        }


        /// <summary>
        /// This function sets the trigger mode to Free Run,
        /// the center frequency to 1.5GHz, 
        /// the span to 40 MHz,
        /// the IQ record length to 1024 samples and
        /// the reference level to 0 dBm.
        /// </summary>
        public void Preset()
        {
            var rs = RSA_API.CONFIG_Preset();
            rs.CheckError(this);
        }

        private void SetRefLevel(double lvl)
        {
             lvl = lvl.Limit(-130,30);
            var rs = RSA_API.CONFIG_SetReferenceLevel(lvl);
            rs.CheckError(this);
        }
        private double GetRefLevel()
        {
            var rs = RSA_API.CONFIG_GetReferenceLevel(out double lvl);
            rs.CheckError(this);
            return lvl;
        }

        private double GetCenterFreq()
        {
            var rs = RSA_API.CONFIG_GetCenterFreq(out double cf);
            rs.CheckError(this);
            return cf;
        }
        private void SetCenterFreq(double cf)
        {
            GetCenterFreqLimits();
            cf = cf.Limit(min_cf, max_cf);
            var rs = RSA_API.CONFIG_SetCenterFreq(cf);
            rs.CheckError(this);
        }

        private double min_cf = double.NaN;
        private double max_cf = double.NaN;
        private void GetCenterFreqLimits()
        {
            if (double.IsNaN(min_cf) || double.IsNaN(max_cf))
            {
                var rs = RSA_API.CONFIG_GetMinCenterFreq(out min_cf);
                rs.CheckError(this);
                rs = RSA_API.CONFIG_GetMaxCenterFreq(out max_cf);
                rs.CheckError(this);
            }
        }
    }

}
