using FluentAssertions;
using HAcgReader.Services;
using HAcgReader.Test.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;

namespace HAcgReader.Test.Services;

/// <summary>
/// 测试 <see cref="DomainService"/>
/// </summary>
[TestClass]
[ExcludeFromCodeCoverage]
public class DomainServiceTest
{
    /// <summary>
    /// 测试 <see cref="DomainService.DomainService"/>
    /// </summary>
    [TestMethod]
    public void TestConstructor()
    {
        using var service = new DomainService();
        service.Should().NotBeNull();
    }

    /// <summary>
    /// 测试 <see cref="DomainService.GetDomain"/>
    /// </summary>
    [DataTestMethod]
    [DataRow(HttpStatusCode.OK, true, "example.com")]
    [DataRow(HttpStatusCode.NotFound, true, "")]
    [DataRow(HttpStatusCode.OK, false, "")]
    public void TestGetDomain(HttpStatusCode statusCode, bool useSampleResponse, string expected)
    {
        var htmlLocation = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            "TestData",
            "TestDomainServicePage.html");
        var htmlContent = File.ReadAllBytes(htmlLocation);

        using var httpResponse = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new ByteArrayContent(useSampleResponse ? htmlContent : Array.Empty<byte>())
        };

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupHttpResponse(HttpMethod.Get, new Uri("https://acg.gy"), httpResponse);

        using var httpClient = new HttpClient(handler.Object);
        using var service = new DomainService(httpClient);

        service.GetDomain().Should().Be(expected);
    }
}
