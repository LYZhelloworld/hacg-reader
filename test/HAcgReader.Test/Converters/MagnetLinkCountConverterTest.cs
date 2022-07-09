using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FluentAssertions;
using HAcgReader.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HAcgReader.Test.Converters;

/// <summary>
/// 测试 <see cref="MagnetLinkCountConverter"/>
/// </summary>
[TestClass]
[ExcludeFromCodeCoverage]
public class MagnetLinkCountConverterTest
{
    /// <summary>
    /// 测试 <see cref="MagnetLinkCountConverter.Convert(object, Type, object, CultureInfo)"/>
    /// </summary>
    [TestMethod]
    public void TestConvert()
    {
        var converter = new MagnetLinkCountConverter();
        var result = converter.Convert(
            new string[] { "1", "2", "3" }, typeof(string), new object(), CultureInfo.InvariantCulture);
        result.Should().Be("3 个链接");

        var action = () => converter.Convert(
            default!, typeof(string), new object(), CultureInfo.InvariantCulture);
        action.Should().Throw<ArgumentNullException>();

        action = () => converter.Convert(
            Array.Empty<string>(), typeof(object), new object(), CultureInfo.InvariantCulture);
        action.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// 测试 <see cref="MagnetLinkCountConverter.ConvertBack(object, Type, object, CultureInfo)"/>
    /// </summary>
    [TestMethod]
    public void TestConvertBack()
    {
        var converter = new MagnetLinkCountConverter();
        converter.ConvertBack(string.Empty, typeof(string[]), new object(), CultureInfo.InvariantCulture);
        // 不断言任何内容
    }
}
