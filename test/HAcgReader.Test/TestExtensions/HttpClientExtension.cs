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
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.Method == method && r.RequestUri == uri),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    }
}
