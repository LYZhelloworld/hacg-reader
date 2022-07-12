using HAcgReader.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    /// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c> 时抛出</exception>
    /// <exception cref="ArgumentException"><paramref name="targetType"/> 不为 <see cref="string"/> 类型时抛出</exception>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (targetType != typeof(string))
        {
            throw new ArgumentException(null, nameof(targetType));
        }

        var magnetLinks = ((IEnumerable<string>)value).ToArray();
        return string.Format(culture, Strings.MagnetLinkCount, magnetLinks.Length);
    }

    /// <summary>
    /// 将描述转换成磁链列表（不使用）
    /// </summary>
    /// <param name="value">磁链列表</param>
    /// <param name="targetType">目标类型</param>
    /// <param name="parameter">参数</param>
    /// <param name="culture">当前使用的 <see cref="CultureInfo"/></param>
    /// <returns>（不使用）</returns>
    /// <exception cref="NotImplementedException">（不使用）</exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
