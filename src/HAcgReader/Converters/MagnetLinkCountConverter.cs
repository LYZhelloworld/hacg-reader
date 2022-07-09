using System;
using System.Globalization;
using System.Windows.Data;

namespace HAcgReader.Converters;

/// <summary>
/// 计算磁链个数的转换器类
/// </summary>
public class MagnetLinkCountConverter : IValueConverter
{
    /// <summary>
    /// 将磁链列表转换成个数描述
    /// </summary>
    /// <param name="value">磁链列表</param>
    /// <param name="targetType">目标类型</param>
    /// <param name="parameter">参数</param>
    /// <param name="culture">当前使用的 <see cref="CultureInfo"/></param>
    /// <returns>转换后的描述</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var magnetLinks = (string[])value;
        return $"{magnetLinks.Length} 个链接";
    }

    /// <summary>
    /// 将描述转换成磁链列表（不使用）
    /// </summary>
    /// <param name="value">磁链列表</param>
    /// <param name="targetType">目标类型</param>
    /// <param name="parameter">参数</param>
    /// <param name="culture">当前使用的 <see cref="CultureInfo"/></param>
    /// <returns>（不使用）</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Array.Empty<string>();
    }
}
