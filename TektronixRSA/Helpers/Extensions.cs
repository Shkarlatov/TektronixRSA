using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Tektronix.TekRSA
{
    public static class Extensions
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static bool CheckError(this ReturnStatus status,TekBase src)
        //{
        //    if (status != ReturnStatus.successful)
        //        //return false;
        //        throw new RSAException(status);
        //    return true;
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckError(this ReturnStatus status, TekBase src, [CallerMemberName] string method = "")
        {
            if (status != ReturnStatus.successful)
                src.OnError(status, method);
            return true;
        }
    }

    public static class ComparableExtensions
    {
        public static T Limit<T>(this T value, T min, T max) where T: IComparable<T>
        {
            if (value.CompareTo(max) > 0)
                value = max;
            if (value.CompareTo(min) < 0)
                value = min;
            return value;
        }
    }

    public static class EnumHelpers
    {
        public static T[] ToValueArray<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToArray();
            // Enum.GetValues(typeof(SpectrumWindows)).Cast<SpectrumWindows>();
        }
    }

}
