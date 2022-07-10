namespace HAcgReader.Factories;

/// <summary>
/// HTTP 客户端工厂类
/// </summary>
public interface IHttpClientFactory
{
    /// <summary>
    /// 创造 <see cref="HttpClient"/> 实例
    /// </summary>
    /// <returns><see cref="HttpClient"/> 对象</returns>
    HttpClient Create();
}