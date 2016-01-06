using System;
using System.Globalization;
using System.Windows.Data;

namespace BidAskBox.Library
{
    internal class SymbolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var text = ((Symbol)value).ToString();

            return text.Substring(0, 3) + "/" + text.Substring(3);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
