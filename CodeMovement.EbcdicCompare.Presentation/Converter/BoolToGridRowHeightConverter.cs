using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CodeMovement.EbcdicCompare.Presentation.Converter
{
    [ValueConversion(typeof(bool), typeof(GridLength))]
    public class BoolToGridRowHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? new GridLength(int.Parse(parameter as string), GridUnitType.Pixel) : new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
