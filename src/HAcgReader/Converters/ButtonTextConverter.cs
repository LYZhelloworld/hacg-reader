using System;
using System.Globalization;
using System.Windows.Data;

namespace HAcgReader.Converters;

public class ButtonTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "更多" : "正在获取信息，请稍后……";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (string)value == "更多";
    }
}
