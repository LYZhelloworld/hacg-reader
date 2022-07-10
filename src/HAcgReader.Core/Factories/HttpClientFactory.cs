namespace HAcgReader.Factories;

/// <summary>
/// HTTP 客户端工厂类
/// </summary>
public class HttpClientFactory : IHttpClientFactory
{
    /// <inheritdoc/>
    public HttpClient Create()
    {
        return new HttpClient();
    }
}
