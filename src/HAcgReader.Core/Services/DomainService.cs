using HAcgReader.Factories;
using System.Text.RegularExpressions;

namespace HAcgReader.Services;

/// <summary>
/// 提供与神社域名相关的功能
/// </summary>
public class DomainService : IDomainService
{
    /// <summary>
    /// 用于发布神社新域名的网站
    /// </summary>
    private const string DomainPublisherUrl = "https://acg.gy/";

    /// <summary>
    /// 网页中链接的正则表达式
    /// </summary>
    private static readonly Regex s_linkPattern = new(@"<a href=""https?://([a-zA-Z0-9\\.]*)"">", RegexOptions.Compiled);

    /// <summary>
    /// HTTP 客户端工厂类
    /// </summary>
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// 构造函数
    /// </summary>
    public DomainService()
        : this(new HttpClientFactory())
    {
    }

    /// <summary>
    /// 初始化所有依赖的构造函数
    /// </summary>
    /// <param name="httpClientFactory">HTTP 客户端工厂类</param>
    public DomainService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    /// <inheritdoc/>
    public string GetDomain()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(DomainPublisherUrl));
        request.Headers.AcceptCharset.Add(new("utf-8"));

        using var httpClient = _httpClientFactory.Create();
        var response = httpClient.Send(request);
        if (!response.IsSuccessStatusCode)
        {
            return string.Empty;
        }

        var match = s_linkPattern.Match(response.Content.ReadAsStringAsync().Result);
        if (match.Success)
        {
            return match.Groups[1].Value.Trim();
        }

        return string.Empty;
    }
}
