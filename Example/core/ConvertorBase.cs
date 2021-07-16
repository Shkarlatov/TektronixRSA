using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace libCore
{
    /* Usage:
     * In Xaml: xmlns:conv="clr-namespace:libCore.Helpers"
     *  <Label Content="{Binding Path=Date, Converter={conv:DateTimeToString}}" />
  

    public class DateTimeToString : ConvertorBase<DateTimeToString>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return date.ToShortDateString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


 <Label Content="{Binding Path=Date, Converter={converters:DateConverter Format=ShortString, Calendar=Gregorian}}" />

public class DateTimeToString : ...
{
public override object Convert(...){...}

public string Format{get;set;}
public CalendarType Calendar{get;set;}
}
    */

    public abstract class ConvertorBase : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Must be implemented in inheritor.
        /// </summary>
        public abstract object Convert(object value, Type targetType, object parameter,
            CultureInfo culture);

        /// <summary>
        /// Override if needed.
        /// </summary>
        public virtual object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #region MarkupExtension members

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #endregion
    }
}
