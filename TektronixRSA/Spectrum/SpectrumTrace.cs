using System.Linq;

using Tektronix.TekRSA.Native;

namespace Tektronix.TekRSA
{
    public class SpectrumTrace : TekBase
    {
        private readonly SpectrumTraces _trace;
        private bool _traceEnable;
        private SpectrumDetectors _detector;


        internal SpectrumTrace(SpectrumTraces trace)
        {
            _trace = trace;
        }


        public bool Enable
        {
            get
            {
                GetTraceType( _traceEnable, _detector);
                return _traceEnable;
            }
            set
            {
                _traceEnable = value;
                SetTraceType( _traceEnable, _detector);
            }
        }
        public SpectrumDetectors Detector
        {
            get
            {
                GetTraceType( _traceEnable, _detector);
                return _detector;
            }
            set 
            {
                _detector = value;
                SetTraceType( _traceEnable, _detector);
            }
        }

        public float[] GetData(int count)
        {
            var tmp = new float[count];

            var size = GetTrace(ref tmp);

            return size < count ? tmp.Take(size).ToArray() : tmp;
        }


        private  void GetTraceType( bool enable, SpectrumDetectors detector)
        {
            var rs = RSA_API.SPECTRUM_GetTraceType(_trace, out enable, out detector);
            rs.CheckError(this);
        }
        private  void SetTraceType(bool enable, SpectrumDetectors detector)
        {
            var rs = RSA_API.SPECTRUM_SetTraceType(_trace,enable, detector);
            rs.CheckError(this);
        }
        private  int GetTrace( ref float[] traceData)
        {
            var rs = RSA_API.SPECTRUM_GetTrace(_trace, traceData.Length, traceData, out int count);
            rs.CheckError(this);
            return count;
        }

    }
}
