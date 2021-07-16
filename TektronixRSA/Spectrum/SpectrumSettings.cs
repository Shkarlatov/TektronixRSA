namespace Tektronix.TekRSA
{
    public struct SpectrumSettings
    {
        public double span;
        public double rbw;
        public bool enableVBW;
        public double vbw;
        public int traceLength;                    //  MUST be odd number
        public SpectrumWindows window;
        public SpectrumVerticalUnits verticalUnit;

        //  additional settings return from SPECTRUM_GetSettings()
        public double actualStartFreq;
        public double actualStopFreq;
        public double actualFreqStepSize;
        public double actualRBW;
        public double actualVBW;
        public int actualNumIQSamples;
    }
}
