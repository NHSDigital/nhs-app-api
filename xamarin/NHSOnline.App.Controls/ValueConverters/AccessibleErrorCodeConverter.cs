using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.ValueConverters
{
    public class AccessibleErrorCodeConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string errorCode)
            {
                return Regex.Replace(errorCode, ".{1}", "$0 ");
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}