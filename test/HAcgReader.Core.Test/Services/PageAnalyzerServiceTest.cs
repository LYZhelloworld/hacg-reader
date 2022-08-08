// <copyright file="PageAnalyzerServiceTest.cs" company="Helloworld">
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
    /// 测试 <see cref="PageAnalyzerService"/>
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PageAnalyzerServiceTest
    {
        /// <summary>
        /// 测试 <see cref="PageAnalyzerService.Analyze(ArticleModel, CancellationToken)"/>
        /// </summary>
        [TestMethod]
        public void TestAnalyze()
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
            var newArticle = service.Analyze(article, default);

            article.MagnetLinks.Should().BeEquivalentTo(new string[]
            {
            "magnet:?xt=urn:btih:0123456789abcdef0123456789abcdef01234567",
            "magnet:?xt=urn:btih:9876543210abcdef0123456789abcdef01234567",
            });

            article.Preview.Should().Be(@"Lorem ipsum dolor sit amet,

consectetur adipiscing elit,

sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
0123456789abcdef0123456789abcdef01234567
Ut enim ad minim veniam,

quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
9876543210abcdef0123456789abcdef01234567

Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.

Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.

0123456789abcdef0123456789abcdef01234567");
        }

        /// <summary>
        /// 测试 <see cref="PageAnalyzerService.Analyze(ArticleModel, CancellationToken)"/> 在获取不到指定标签时的情况
        /// </summary>
        [TestMethod]
        public void TestAnalyzeTagNotFound()
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
            var newArticle = service.Analyze(article, default);

            article.MagnetLinks.Should().BeEquivalentTo(Array.Empty<string>());
        }

        /// <summary>
        /// 测试 <see cref="PageAnalyzerService.Analyze(ArticleModel, CancellationToken)"/> 在 HTTP 请求失败时的情况
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
            var newArticle = service.Analyze(article, default);

            article.MagnetLinks.Should().BeEquivalentTo(Array.Empty<string>());
        }

        /// <summary>
        /// 测试 <see cref="PageAnalyzerService.Analyze(ArticleModel, CancellationToken)"/> 在参数为 <see langword="null"/> 时的情况
        /// </summary>
        [TestMethod]
        public void TestAnalyzeAsyncNullArgument()
        {
            var service = new PageAnalyzerService();
            var action = () => service.Analyze(null!, default);
            action.Should().Throw<ArgumentNullException>();
        }
    }
}