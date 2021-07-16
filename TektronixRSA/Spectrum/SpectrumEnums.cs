namespace Tektronix.TekRSA
{
    public enum SpectrumWindows
    {
        Kaiser=0,
        Mil6dB=1,
        BlackmanHarris=2,
        Rectangle=3,
        FlatTop=4,
        Hann=5
    }

    public enum SpectrumTraces
    {
        Trace1=0,
        Trace2=1,
        Trace3=2
    }

    public enum SpectrumDetectors
    {
        PosPeak=0,
        NegPeak=1,
        AvgVRMS=2,
        Sample=3
    }

    public enum SpectrumVerticalUnits
    {
        dBm=0,
        Watt=1,
        Volt=2,
        Amp=3,
        dBmV=4
    }
}
