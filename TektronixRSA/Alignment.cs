using Tektronix.TekRSA.Native;

namespace Tektronix.TekRSA
{
    public class Alignment : TekBase
    {
        internal Alignment() { }

        /// <summary>
        /// Runs the device alignment process
        /// </summary>
        public void Run()
        {
            var rs = RSA_API.ALIGN_RunAlignment();
            rs.CheckError(this);
        }
        /// <summary>
        /// Reports device warm-up status.
        /// True indicates the device's warm-up interval has been reached.
        /// False indicates the warm-up interval has not been reached.
        /// </summary>
        public bool Status => WarmupStatus();
        /// <summary>
        /// Determines if an alignment is needed or not.
        ///  True indicates an alignment is needed.
        ///  False indicates an alignment is not needed.
        /// </summary>
        public bool IsNeeded => AlignmentNeeded();

        private bool WarmupStatus()
        {
            var rs = RSA_API.ALIGN_GetWarmupStatus(out bool status);
            rs.CheckError(this);
            return status;
        }
        private bool AlignmentNeeded()
        {
            var rs = RSA_API.ALIGN_GetAlignmentNeeded(out bool status);
            rs.CheckError(this);
            return status;
        }
    }

}
