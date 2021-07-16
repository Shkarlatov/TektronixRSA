namespace Tektronix.TekRSA
{
    public struct SpectrumTraceInfo
    {
        ulong timestamp;            //  timestamp of the first acquisition sample
        DataStatus acqDataStatus;	// See AcqDataStatus enumeration for bit definitions
    }

    public enum DataStatus : ushort
    {
        ok = 0x0,
        adcOverrange = 0x1,
        refFreqUnlock = 0x2,
        adcDataLost = 0x20
    }
}
