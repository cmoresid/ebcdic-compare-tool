using System;
using System.Windows.Data;

namespace CodeMovement.EbcdicCompare.Presentation.Converter
{
    public class NullableBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;

            if (value is bool)
            {
                return (bool)value;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;

            if (value is bool)
            {
                return (bool)value;
            }

            return value;
        }

        #endregion
    }
}
