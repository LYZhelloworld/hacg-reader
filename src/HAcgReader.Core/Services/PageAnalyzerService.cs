using HAcgReader.Factories;
using HAcgReader.Models;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace HAcgReader.Services;

/// <summary>
/// 分析页面内容，寻找磁链
/// </summary>
public class PageAnalyzerService : IPageAnalyzerService
{
    /// <summary>
    /// 磁链前缀
    /// </summary>
    private const string MagnetPrefix = "magnet:?xt=urn:btih:";

    /// <summary>
    /// 磁链哈希的正则表达式
    /// </summary>
    private static readonly Regex s_magnetLink = new(@"(?<![0-9a-fA-F])([0-9a-fA-F]{40})(?![0-9a-fA-F])", RegexOptions.Compiled);

    /// <summary>
    /// HTTP 客户端工厂类
    /// </summary>
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// 构造函数
    /// </summary>
    public PageAnalyzerService()
        : this(new HttpClientFactory())
    {
    }

    /// <summary>
    /// 初始化所有依赖的构造函数
    /// </summary>
    /// <param name="httpClientFactory">HTTP 客户端工厂类</param>
    public PageAnalyzerService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    /// <inheritdoc/>
    public async Task<ArticleModel> AnalyzeAsync(ArticleModel article)
    {
        if (article == null)
        {
            throw new ArgumentNullException(nameof(article));
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, article.Link);
        request.Headers.AcceptCharset.Add(new("utf-8"));
        using var httpClient = _httpClientFactory.Create();
        var response = await httpClient.SendAsync(request).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            // 无法解析
            return article;
        }

        article.MagnetLinks = Parse(await response.Content!.ReadAsStringAsync(default).ConfigureAwait(false));
        return article;
    }

    /// <summary>
    /// 解析返回的内容
    /// </summary>
    /// <param name="content">返回内容</param>
    /// <returns>解析出的磁链集合</returns>
    private static IEnumerable<string> Parse(string content)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(content);

        // 寻找 <div class="entry-content">
        var entryContentTags = doc.DocumentNode.SelectNodes("//div[@class=\"entry-content\"]");
        if (entryContentTags == null)
        {
            // 没有找到对应标签
            return Array.Empty<string>();
        }

        return entryContentTags.AsEnumerable()
            .Select(entryContentTag => entryContentTag.InnerHtml)
            .Select(innerHtml =>
            {
                var matches = s_magnetLink.Matches(innerHtml);
                return matches.Select(match => match.Groups[1].Value).Distinct();
            })
            .SelectMany(i => i)
            .Select(i => MagnetPrefix + i);
    }
}
