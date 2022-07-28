// <copyright file="DomainServiceTest.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.Core.Test.Services
{
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Reflection;
    using FluentAssertions;
    using HAcgReader.Core.Services;
    using HAcgReader.Core.Test.TestExtensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// 测试 <see cref="DomainService"/>
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DomainServiceTest
    {
        /// <summary>
        /// 测试 <see cref="DomainService()"/>
        /// </summary>
        [TestMethod]
        public void TestConstructor()
        {
            var service = new DomainService();
            service.Should().NotBeNull();
        }

        /// <summary>
        /// 测试 <see cref="DomainService.GetDomain"/>
        /// </summary>
        /// <param name="statusCode">状态码</param>
        /// <param name="useSampleResponse">是否使用样例应答</param>
        /// <param name="expected">期待的结果</param>
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
                Content = new ByteArrayContent(useSampleResponse ? htmlContent : Array.Empty<byte>()),
            };

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupHttpResponse(HttpMethod.Get, new Uri("http://acg.gy"), httpResponse);

            var service = new DomainService(handler.GetHttpClientFactory());

            service.GetDomain().Should().Be(expected);
        }
    }
}