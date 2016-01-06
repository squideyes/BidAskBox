using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BidAskBox.Library
{
    internal class DeltaToStringConverter : IValueConverter
    {
        private readonly Geometry downArrow = Geometry.Parse("M0,0 L1,0 0.5,1Z");
        private readonly Geometry upArrow = Geometry.Parse("M0,1 L1,1 0.5,0Z");
        private readonly Geometry noneArrow = Geometry.Empty;

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            switch ((Delta)value)
            {
                case Delta.Up:
                    return upArrow;
                case Delta.Down:
                    return downArrow;
                default:
                    return noneArrow;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
