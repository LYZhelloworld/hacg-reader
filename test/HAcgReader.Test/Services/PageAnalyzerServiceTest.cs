using FluentAssertions;
using HAcgReader.Models;
using HAcgReader.Services;
using HAcgReader.Test.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection;

namespace HAcgReader.Test.Services;

/// <summary>
/// 测试 <see cref="PageAnalyzerService"/>
/// </summary>
[TestClass]
[ExcludeFromCodeCoverage]
public class PageAnalyzerServiceTest
{
    /// <summary>
    /// 测试 <see cref="PageAnalyzerService.AnalyzeAsync(ArticleModel)"/>
    /// </summary>
    [TestMethod]
    public void TestAnalyzeAsync()
    {
        var htmlLocation = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            "TestData",
            "TestPage.html");
        var htmlContent = File.ReadAllBytes(htmlLocation);

        using var httpResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new ByteArrayContent(htmlContent),
        };

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupHttpResponse(HttpMethod.Get, new Uri("https://example.com/wp/00001.html"), httpResponse);

        var service = new PageAnalyzerService(handler.GetHttpClientFactory());

        var article = new ArticleModel()
        {
            Link = "https://example.com/wp/00001.html",
        };
        var newArticle = service.AnalyzeAsync(article);

        article.MagnetLinks.Should().BeEquivalentTo(new string[]
        {
            "magnet:?xt=urn:btih:0123456789abcdef0123456789abcdef01234567",
            "magnet:?xt=urn:btih:9876543210abcdef0123456789abcdef01234567",
        });
    }

    /// <summary>
    /// 测试 <see cref="PageAnalyzerService.AnalyzeAsync(ArticleModel)"/> 在获取不到指定标签时的情况
    /// </summary>
    [TestMethod]
    public void TestAnalyzeAsyncTagNotFound()
    {
        using var httpResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(string.Empty),
        };

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupHttpResponse(HttpMethod.Get, new Uri("https://example.com/wp/00001.html"), httpResponse);

        var service = new PageAnalyzerService(handler.GetHttpClientFactory());

        var article = new ArticleModel()
        {
            Link = "https://example.com/wp/00001.html",
        };
        var newArticle = service.AnalyzeAsync(article);

        article.MagnetLinks.Should().BeEquivalentTo(Array.Empty<string>());
    }

    /// <summary>
    /// 测试 <see cref="PageAnalyzerService.AnalyzeAsync(ArticleModel)"/> 在 HTTP 请求失败时的情况
    /// </summary>
    [TestMethod]
    public void TestAnalyzeAsyncHttpErrorResponse()
    {
        using var httpResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent(string.Empty),
        };

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupHttpResponse(HttpMethod.Get, new Uri("https://example.com/wp/00001.html"), httpResponse);

        var service = new PageAnalyzerService(handler.GetHttpClientFactory());

        var article = new ArticleModel()
        {
            Link = "https://example.com/wp/00001.html",
        };
        var newArticle = service.AnalyzeAsync(article);

        article.MagnetLinks.Should().BeEquivalentTo(Array.Empty<string>());
    }

    /// <summary>
    /// 测试 <see cref="PageAnalyzerService.AnalyzeAsync(ArticleModel)"/> 在参数为 <c>null</c> 时的情况
    /// </summary>
    [TestMethod]
    public void TestAnalyzeAsyncNullArgument()
    {
        var service = new PageAnalyzerService();
        var action = () => service.AnalyzeAsync(null!);
        action.Should().ThrowAsync<ArgumentNullException>().Wait();
    }
}
