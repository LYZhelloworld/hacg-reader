using HAcgReader.Factories;
using Moq;
using Moq.Protected;
using System.Diagnostics.CodeAnalysis;

namespace HAcgReader.Test.TestHelpers;

/// <summary>
/// <see cref="HttpClient"/> 的 <see cref="Mock{T}"/> 对象的辅助类
/// </summary>
[ExcludeFromCodeCoverage]
public static class HttpClientExtension
{
    /// <summary>
    /// 设置 HTTP 返回内容
    /// </summary>
    /// <param name="mock"><see cref="Mock{T}"/> 对象</param>
    /// <param name="method">HTTP 方法</param>
    /// <param name="uri">HTTP URI</param>
    /// <param name="response">返回内容</param>
    public static void SetupHttpResponse(this Mock<HttpMessageHandler> mock,
        HttpMethod method, Uri uri, HttpResponseMessage response)
    {
        mock.Protected()
            .Setup<HttpResponseMessage>(
                "Send",
                ItExpr.Is<HttpRequestMessage>(r => r.Method == method && r.RequestUri == uri),
                ItExpr.IsAny<CancellationToken>())
            .Returns(response);
        mock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.Method == method && r.RequestUri == uri),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    }

    /// <summary>
    /// 获取加载了测试 <see cref="HttpMessageHandler"/> 的 HTTP 客户端工厂类
    /// </summary>
    /// <param name="mock"><see cref="HttpMessageHandler"/> 的 <see cref="Mock{T}"/> 对象</param>
    /// <returns>HTTP 客户端工厂类</returns>
    public static IHttpClientFactory GetHttpClientFactory(this Mock<HttpMessageHandler> mock)
    {
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.Create()).Returns(() => new HttpClient(mock.Object));
        return httpClientFactory.Object;
    }
}
