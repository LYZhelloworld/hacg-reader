using FluentAssertions;
using HAcgReader.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;

namespace HAcgReader.Test.Services;

/// <summary>
/// 测试 <see cref="DomainService"/>
/// </summary>
[TestClass]
[ExcludeFromCodeCoverage]
public class DomainServiceTest
{
    private const string SampleResponse = @"
<!DOCTYPE html>
<html>
<head>
<meta charset=""UTF-8"">
<title>琉璃神社★分享动漫快乐</title>
<body>
<div>
<p><a href=""https://example.com"">https://example.com</a></p>
<p>由于经常被不明人士攻击，神社会定时更换地址。</p>
<p>大家可以收藏这个页面到书签。</p>
<p>希望大家不要再被骗。</p>
<p>ACG.GY</p>
<p>ACG.公益</p>
</div>
";

    /// <summary>
    /// 测试 <see cref="DomainService.GetDomain"/>
    /// </summary>
    [TestMethod]
    public void TestGetDomain()
    {
        using var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(SampleResponse, Encoding.UTF8, "text/html")
        };

        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(
                    r => r.Method == HttpMethod.Get &&
                    r.RequestUri!.ToString().StartsWith("https://acg.gy/")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        using var httpClient = new HttpClient(handler.Object);
        using var service = new DomainService(httpClient);

        service.GetDomain().Should().Be("example.com");
    }

    /// <summary>
    /// 测试 <see cref="DomainService.GetDomain"/> 获取失败的情况
    /// </summary>
    [TestMethod]
    public void TestGetDomainFailed()
    {
        using var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(string.Empty, Encoding.UTF8, "text/html")
        };

        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(
                    r => r.Method == HttpMethod.Get &&
                    r.RequestUri!.ToString().StartsWith("https://acg.gy/")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        using var httpClient = new HttpClient(handler.Object);
        using var service = new DomainService(httpClient);

        service.GetDomain().Should().Be(string.Empty);
    }
}
