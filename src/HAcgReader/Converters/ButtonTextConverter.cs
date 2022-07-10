using System;
using System.Globalization;
using System.Windows.Data;

namespace HAcgReader.Converters;

/// <summary>
/// 拉取按钮文本转换类
/// </summary>
public class ButtonTextConverter : IValueConverter
{
    /// <summary>
    /// 将代表按钮有效性的布尔值转换成文本
    /// </summary>
    /// <param name="value">按钮有效性</param>
    /// <param name="targetType">目标类型</param>
    /// <param name="parameter">参数</param>
    /// <param name="culture">当前使用的 <see cref="CultureInfo"/></param>
    /// <returns>按钮文本</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "更多" : "正在获取信息，请稍后……";
    }

    /// <summary>
    /// 将文本转换成代表按钮有效性的布尔值
    /// </summary>
    /// <param name="value">按钮文本</param>
    /// <param name="targetType">目标类型</param>
    /// <param name="parameter">参数</param>
    /// <param name="culture">当前使用的 <see cref="CultureInfo"/></param>
    /// <returns>按钮有效性</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (string)value == "更多";
    }
}
