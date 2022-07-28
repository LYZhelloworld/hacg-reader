// <copyright file="MagnetLinkCountConverter.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using HAcgReader.Resources;

    /// <summary>
    /// 计算磁链个数的转换器类
    /// </summary>
    public class MagnetLinkCountConverter : IValueConverter
    {
        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}