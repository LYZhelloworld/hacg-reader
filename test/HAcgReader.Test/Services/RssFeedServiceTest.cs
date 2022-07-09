using FluentAssertions;
using HAcgReader.Models;
using HAcgReader.Services;
using HAcgReader.Test.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Reflection;

namespace HAcgReader.Test.Services;

[TestClass]
public class RssFeedServiceTest
{
    [TestMethod]
    public void TestFetchNextAsync()
    {
        var xmlLocation = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            "TestData",
            "TestRssFeedPage.xml");
        var xmlContent = File.ReadAllBytes(xmlLocation);

        using var httpResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new ByteArrayContent(xmlContent),
        };
        using var errorHttpResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.NotFound,
        };

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupHttpResponse(HttpMethod.Get, new Uri("https://example.com/wp/feed"), httpResponse);
        handler.SetupHttpResponse(HttpMethod.Get, new Uri("https://example.com/wp/feed?paged=2"), errorHttpResponse);

        using var httpClient = new HttpClient(handler.Object);
        using var service = new RssFeedService("example.com", httpClient);

        var pages = service.FetchNextAsync();
        pages.Result.Should().BeEquivalentTo(new ArticleModel[]
        {
            new()
            {
                Title = "Item 1",
                Link = "https://example.com/wp/00001.html",
                CommentLink = "https://example.com/wp/00001.html#respond",
                Creator = "creator 1",
                PubDate = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Categories = new string[] { "Cat 1", "Cat 2", "Cat 3"},
                CommentCount = 0,
            },
            new()
            {
                Title = "Item 2",
                Link = "https://example.com/wp/00002.html",
                CommentLink = "https://example.com/wp/00002.html#respond",
                Creator = "creator 2",
                PubDate = new DateTime(2022, 1, 2, 8, 0, 0, DateTimeKind.Utc),
                Categories = new string[] { "Cat 4"},
                CommentCount = 50,
            },
        });

        // 下一页应为空
        pages = service.FetchNextAsync();
        pages.Result.Should().BeEmpty();
    }
}
