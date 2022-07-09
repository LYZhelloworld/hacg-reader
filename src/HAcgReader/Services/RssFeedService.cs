using HAcgReader.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace HAcgReader.Services;

/// <summary>
/// 获取神社 RSS Feed
/// </summary>
public sealed class RssFeedService : IDisposable
{
    /// <summary>
    /// RSS Feed 链接格式
    /// </summary>
    private const string UriFormat = "https://{0}/wp/feed";

    private const string DateTimeFormat = "ddd, dd MMM yyyy HH:mm:ss zzz";

    /// <summary>
    /// 神社 RSS Feed 链接
    /// </summary>
    private readonly string _path;

    /// <summary>
    /// HTTP 客户端
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// RSS Feed 页数
    /// </summary>
    private int _page = 1;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="domain">神社域名</param>
    /// <exception cref="ArgumentException">域名为空时抛出</exception>
    public RssFeedService(string domain)
        : this(domain, new HttpClient())
    {
    }

    /// <summary>
    /// 初始化所有依赖的构造函数
    /// </summary>
    /// <param name="domain">神社域名</param>
    /// <param name="httpClient">HTTP 客户端</param>
    /// <exception cref="ArgumentException">域名为空时抛出</exception>
    public RssFeedService(string domain, HttpClient httpClient)
    {
        if (string.IsNullOrEmpty(domain))
        {
            throw new ArgumentException(null, nameof(domain));
        }

        _path = string.Format(CultureInfo.InvariantCulture, UriFormat, domain);
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<ArticleModel>> FetchNextAsync()
    {
        var uri = new UriBuilder(_path);
        if (_page > 1)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("paged", _page.ToString(CultureInfo.InvariantCulture));
            uri.Query = query.ToString();
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, uri.Uri);
        request.Headers.AcceptCharset.Add(new("utf-8"));
        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            // 已无更新的内容，不必自增页数
            return Array.Empty<ArticleModel>();
        }

        if (response.Content == null)
        {
            return Array.Empty<ArticleModel>();
        }

        var result = Parse(await response.Content.ReadAsStreamAsync(default).ConfigureAwait(false));
        _page++;
        return result;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _httpClient.Dispose();
    }

    private static IEnumerable<ArticleModel> Parse(Stream content)
    {
        XmlDocument doc = new();
        doc.Load(content);

        var root = doc.DocumentElement;
        if (root == null)
        {
            return Array.Empty<ArticleModel>();
        }

        var items = root.SelectNodes("channel/item");
        if (items == null)
        {
            return Array.Empty<ArticleModel>();
        }

        var namespaceManager = new XmlNamespaceManager(doc.NameTable);
        namespaceManager.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
        namespaceManager.AddNamespace("slash", "http://purl.org/rss/1.0/modules/slash/");

        return items.Cast<XmlNode>().Select(item =>
        {
            var article = new ArticleModel()
            {
                Title = item.SelectSingleNode("title")?.InnerText ?? string.Empty,
                Link = item.SelectSingleNode("link")?.InnerText ?? string.Empty,
                CommentLink = item.SelectSingleNode("comments")?.InnerText ?? string.Empty,
                Creator = item.SelectSingleNode("dc:creator", namespaceManager)?.InnerText ?? string.Empty,
            };

            var pubDateStr = item.SelectSingleNode("pubDate")?.InnerText ?? string.Empty;
            if (TryParseDateTime(pubDateStr, out var pubDate))
            {
                article.PubDate = pubDate;
            }

            var commentCountStr = item.SelectSingleNode("slash:comments", namespaceManager)?.InnerText;
            if (int.TryParse(commentCountStr, out var commentCount))
            {
                article.CommentCount = commentCount;
            }

            var categories = item.SelectNodes("category")?.Cast<XmlNode>()
                                 .Select(categoryItem => categoryItem.InnerText);
            article.Categories = categories ?? Array.Empty<string>();

            return article;
        });
    }

    private static bool TryParseDateTime(string dateTimeStr, out DateTime dateTime)
    {
        // 由于无法解析四个数字的时区，所以先转换一下，将其从 +0000 变为 +00:00 这样的时区。
        dateTimeStr = Regex.Replace(dateTimeStr, @"(\+|-)(\d\d)(\d\d)$", "$1$2:$3");
        return DateTime.TryParseExact(
            dateTimeStr,
            DateTimeFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal,
            out dateTime);
    }
}
