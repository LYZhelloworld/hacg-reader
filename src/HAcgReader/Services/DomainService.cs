using System;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace HAcgReader.Services;

/// <summary>
/// 提供与神社域名相关的功能
/// </summary>
public sealed class DomainService : IDomainService, IDisposable
{
    /// <summary>
    /// 用于发布神社新域名的网站
    /// </summary>
    private const string DomainPublisherUrl = "https://acg.gy/";

    /// <summary>
    /// 网页中链接的正则表达式
    /// </summary>
    private const string LinkPattern = @"<a href=""https?://([a-zA-Z0-9\\.]*)"">";

    /// <summary>
    /// 编译好的正则表达式
    /// </summary>
    private static readonly Regex s_linkPattern = new(LinkPattern, RegexOptions.Compiled);

    /// <summary>
    /// HTTP 客户端
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// 构造函数
    /// </summary>
    public DomainService()
        : this(new HttpClient())
    {
    }

    /// <summary>
    /// 初始化所有依赖的构造函数
    /// </summary>
    /// <param name="httpClient">HTTP 客户端</param>
    public DomainService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public string GetDomain()
    {
        var response = _httpClient.GetStringAsync(new Uri(DomainPublisherUrl)).Result;
        var match = s_linkPattern.Match(response);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return string.Empty;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
