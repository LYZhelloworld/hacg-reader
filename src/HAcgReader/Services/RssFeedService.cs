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

    /// <summary>
    /// 日期格式
    /// </summary>
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

    /// <summary>
    /// 异步获取下一页 RSS Feed 内容
    /// </summary>
    /// <returns>异步获取的下一页内容</returns>
    public async Task<IEnumerable<ArticleModel>> FetchNextAsync()
    {
        var uri = new UriBuilder(_path);

        // 页数大于 1 时添加“paged=页数”
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
            // 返回 404，已无更多内容，不必自增页数
            return Array.Empty<ArticleModel>();
        }

        var result = Parse(await response.Content!.ReadAsStreamAsync(default).ConfigureAwait(false));
        _page++;
        return result;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _httpClient.Dispose();
    }

    /// <summary>
    /// 解析返回的内容
    /// </summary>
    /// <param name="content">返回内容的 <see cref="Stream"/></param>
    /// <returns>解析出的 <see cref="ArticleModel"/> 集合</returns>
    private static IEnumerable<ArticleModel> Parse(Stream content)
    {
        XmlDocument doc = new();
        try
        {
            doc.Load(content);
        }
        catch (XmlException)
        {
            return Array.Empty<ArticleModel>();
        }

        // root 此时应为 <rss>
        var root = doc.DocumentElement!;

        // items 应为 <channel> 下的所有 <item> 标签
        var items = root.SelectNodes("channel/item")!;

        // 为 <dc:*> 和 <slash:*> 添加前缀，否则无法识别
        var namespaceManager = new XmlNamespaceManager(doc.NameTable);
        namespaceManager.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
        namespaceManager.AddNamespace("slash", "http://purl.org/rss/1.0/modules/slash/");

        return items.Cast<XmlNode>().Select(item =>
        {
            // magnetLinks 留给其它服务识别，这里忽略
            var article = new ArticleModel()
            {
                // <title>
                Title = item.SelectSingleNode("title")?.InnerText ?? string.Empty,

                // <link>
                Link = item.SelectSingleNode("link")?.InnerText ?? string.Empty,

                // <comments>
                CommentLink = item.SelectSingleNode("comments")?.InnerText ?? string.Empty,

                // <dc:creator>
                Creator = item.SelectSingleNode("dc:creator", namespaceManager)?.InnerText ?? string.Empty,
            };

            // <pubDate>
            var pubDateStr = item.SelectSingleNode("pubDate")?.InnerText ?? string.Empty;
            if (TryParseDateTime(pubDateStr, out var pubDate))
            {
                article.PubDate = pubDate;
            }

            // <slash:comments>
            var commentCountStr = item.SelectSingleNode("slash:comments", namespaceManager)?.InnerText;
            if (int.TryParse(commentCountStr, out var commentCount))
            {
                article.CommentCount = commentCount;
            }

            // <category>，有多个
            article.Categories = item.SelectNodes("category")!
                                     .Cast<XmlNode>()
                                     .Select(categoryItem => categoryItem.InnerText);

            return article;
        });
    }

    /// <summary>
    /// 解析日期和时间
    /// </summary>
    /// <param name="dateTimeStr">字符串表示的日期和时间</param>
    /// <param name="dateTime">解析结果</param>
    /// <returns><c>true</c> 表示解析成功，<c>false</c> 表示失败</returns>
    /// <remarks>
    /// 由于 RSS 返回的日期为类似于 <c>Sat, 01 Jan 2022 00:00:00 +0000</c> 这样的结果，
    /// 其中的时区无法被标准库识别，所以需要进行处理，为其加上“<c>:</c>”。
    /// </remarks>
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
