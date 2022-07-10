using FluentAssertions;
using HAcgReader.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace HAcgReader.Test.Factories;

/// <summary>
/// 测试 <see cref="HttpClientFactory"/>
/// </summary>
[TestClass]
[ExcludeFromCodeCoverage]
public class HttpClientFactoryTest
{
    /// <summary>
    /// 测试 <see cref="HttpClientFactory.Create"/>
    /// </summary>
    [TestMethod]
    public void TestCreate()
    {
        var factory = new HttpClientFactory();
        using var httpClient = factory.Create();
        httpClient.Should().NotBeNull();
    }
}
