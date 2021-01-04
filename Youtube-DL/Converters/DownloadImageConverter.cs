using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Youtube_DL.Converters
{
    class DownloadImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            new BitmapImage(new Uri($"pack://application:,,,/Youtube-DL;component/Resources/{value}.png"));

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
