using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace BidAskBox.Library
{
    internal class DeltaToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            switch ((Delta)value)
            {
                case Delta.Up:
                    return GetBrush("#5BBA00");
                case Delta.Down:
                    return GetBrush("#E81515");
                default:
                    return GetBrush("#A9A9A9"); 
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

        private static object GetBrush(string color)
        {
            var bc = new BrushConverter();

            var brush = (Brush)bc.ConvertFrom(color);

            if (brush is Freezable)
                return (brush as Freezable).Clone();

            return brush;
        }

        public static DeltaToBrushConverter Instance = new DeltaToBrushConverter();
    }
}
