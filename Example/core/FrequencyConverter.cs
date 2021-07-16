using System;
using System.Globalization;


namespace libCore
{
    public enum FrequencyGrid
    {
        Hz=0,
        kHz= (int)1e3,
        MHz= (int)1e6,
        GHz= (int)1e9
    }
    public class FrequencyConverter : ConvertorBase
    {
        public FrequencyGrid FrequencyGrid { get; set; }


        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value = Hz
            if (value == null) return 0;
            return (double)value / (double)FrequencyGrid;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //value in FreqGrid
            //return (double)value * (double)FrequencyGrid;
            var s = (string)value;
            double f;
            double? Result;
            if (double.TryParse(s, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out f))
                Result = f;
            else if (double.TryParse(s, out f))
                Result = f;
            else
                Result = 0;
            return Result * (double)FrequencyGrid;
        }
    }
}
