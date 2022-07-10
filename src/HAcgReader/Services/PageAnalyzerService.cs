using HAcgReader.Models;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HAcgReader.Services;

/// <summary>
/// 分析页面内容，寻找磁链
/// </summary>
public sealed class PageAnalyzerService : IPageAnalyzerService, IDisposable
{
    /// <summary>
    /// 磁链哈希的正则表达式
    /// </summary>
    private static readonly Regex s_magnetLink = new(@"(?<![0-9a-fA-F])([0-9a-fA-F]{40})(?![0-9a-fA-F])", RegexOptions.Compiled);

    /// <summary>
    /// HTTP 客户端
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// 构造函数
    /// </summary>
    public PageAnalyzerService()
        : this(new HttpClient())
    {
    }

    /// <summary>
    /// 初始化所有依赖的构造函数
    /// </summary>
    /// <param name="httpClient">HTTP 客户端</param>
    public PageAnalyzerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            // 无法解析
            return article;
        }

        article.MagnetLinks = Parse(await response.Content!.ReadAsStringAsync(default).ConfigureAwait(false));
        return article;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _httpClient.Dispose();
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
            .SelectMany(i => i);
    }
}
