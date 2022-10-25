// <copyright file="RssFeedServiceTest.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.Core.Test.Services
{
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Reflection;
    using FluentAssertions;
    using HAcgReader.Core.Models;
    using HAcgReader.Core.Services;
    using HAcgReader.Core.Test.TestExtensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// 测试 <see cref="RssFeedService"/>
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class RssFeedServiceTest
    {
        /// <summary>
        /// 测试用 XML 头
        /// </summary>
        private const string TestXmlHeader = @"<?xml version=""1.0"" encoding=""UTF-8""?>";

        /// <summary>
        /// 测试 <see cref="RssFeedService(string)"/>
        /// </summary>
        [TestMethod]
        public void TestConstructor()
        {
            var service = new RssFeedService("example.com");
            service.Should().NotBeNull();

            var action = () => { var service = new RssFeedService(string.Empty); };
            action.Should().Throw<ArgumentException>();
        }

        /// <summary>
        /// 测试 <see cref="RssFeedService.FetchNext(CancellationToken)"/>
        /// </summary>
        [TestMethod]
        public void TestFetchNext()
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

            var service = new RssFeedService("example.com", handler.GetHttpClientFactory());

            var pages = service.FetchNext(default);
            pages.Should().BeEquivalentTo(new ArticleModel[]
            {
            new()
            {
                Title = "Item 1",
                Link = "https://example.com/wp/00001.html",
                CommentLink = "https://example.com/wp/00001.html#respond",
                Creator = "creator 1",
                PubDate = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Categories = new string[] { "Cat 1", "Cat 2", "Cat 3" },
                CommentCount = 0,
            },
            new()
            {
                Title = "Item 2",
                Link = "https://example.com/wp/00002.html",
                CommentLink = "https://example.com/wp/00002.html#respond",
                Creator = "creator 2",
                PubDate = new DateTime(2022, 1, 2, 8, 0, 0, DateTimeKind.Utc),
                Categories = new string[] { "Cat 4" },
                CommentCount = 50,
            },
            });

            // 下一页应为空
            pages = service.FetchNext(default);
            pages.Should().BeEmpty();
        }

        /// <summary>
        /// 测试 <see cref="RssFeedService.FetchNext(CancellationToken)"/> 在根标签或者 <c>&lt;channel&gt;</c> 标签丢失时的情况
        /// </summary>
        /// <param name="content">测试 XML 的内容</param>
        [DataTestMethod]
        [DataRow("")]
        [DataRow(@"<rss/>")]
        public void TestFetchNextEmptyTags(string content)
        {
            using var httpResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(TestXmlHeader + content),
            };

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupHttpResponse(HttpMethod.Get, new Uri("https://example.com/wp/feed"), httpResponse);

            var service = new RssFeedService("example.com", handler.GetHttpClientFactory());

            var pages = service.FetchNext(default);
            pages.Should().BeEquivalentTo(Array.Empty<ArticleModel>());
        }

        /// <summary>
        /// 测试 <see cref="RssFeedService.FetchNext(CancellationToken)"/> 在 <c>&lt;item&gt;</c> 下各个标签为空时的情况
        /// </summary>
        [TestMethod]
        public void TestFetchNextAsyncEmptyTagsInItem()
        {
            using var httpResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(TestXmlHeader + @"<rss><channel><item/></channel></rss>"),
            };

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupHttpResponse(HttpMethod.Get, new Uri("https://example.com/wp/feed"), httpResponse);

            var service = new RssFeedService("example.com", handler.GetHttpClientFactory());

            var pages = service.FetchNext(default);
            pages.Should().BeEquivalentTo(new ArticleModel[] { new() });
        }
    }
}
