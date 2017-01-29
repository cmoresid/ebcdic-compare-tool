using System;
using System.Windows.Data;
using CodeMovement.EbcdicCompare.Models;

namespace CodeMovement.EbcdicCompare.Presentation.Converter
{
    [ValueConversion(typeof(RecordFlag), typeof(string))]
    public class FlagToIconConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            if (!(value is RecordFlag)) 
                return value;
            
            var flagValue = (RecordFlag) value;

            switch (flagValue)
            {
                case RecordFlag.Different:
                    return "pack://application:,,,/Images/red.png";
                case RecordFlag.Identical:
                    return "pack://application:,,,/Images/green.png";
                case RecordFlag.Extra:
                    return "pack://application:,,,/Images/extra.png";
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}