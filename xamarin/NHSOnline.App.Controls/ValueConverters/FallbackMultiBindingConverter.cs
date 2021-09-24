using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.ValueConverters
{
    public class FallbackMultiBindingConverter: IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.ToList().FirstOrDefault(x => x != null) ?? "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}