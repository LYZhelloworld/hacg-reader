// <copyright file="RssFeedService.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.Core.Services
{
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Xml;
    using HAcgReader.Core.Factories;
    using HAcgReader.Core.Models;

    /// <summary>
    /// 获取神社 RSS Feed
    /// </summary>
    public class RssFeedService : IRssFeedService
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
        private readonly string path;

        /// <summary>
        /// HTTP 客户端工厂类
        /// </summary>
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// RSS Feed 页数
        /// </summary>
        private int page = 1;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="domain">神社域名</param>
        /// <exception cref="ArgumentException">域名为空时抛出</exception>
        public RssFeedService(string domain)
            : this(domain, new HttpClientFactory())
        {
        }

        /// <summary>
        /// 初始化所有依赖的构造函数
        /// </summary>
        /// <param name="domain">神社域名</param>
        /// <param name="httpClientFactory">HTTP 客户端工厂类</param>
        /// <exception cref="ArgumentException">域名为空时抛出</exception>
        public RssFeedService(string domain, IHttpClientFactory httpClientFactory)
        {
            if (string.IsNullOrEmpty(domain))
            {
                throw new ArgumentException(null, nameof(domain));
            }

            this.path = string.Format(CultureInfo.InvariantCulture, UriFormat, domain);
            this.httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc/>
        public IEnumerable<ArticleModel> FetchNext(CancellationToken cancellationToken)
        {
            var uri = new UriBuilder(this.path);

            // 页数大于 1 时添加“paged=页数”
            if (this.page > 1)
            {
                var query = HttpUtility.ParseQueryString(string.Empty);
                query.Add("paged", this.page.ToString(CultureInfo.InvariantCulture));
                uri.Query = query.ToString();
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, uri.Uri);
            using var handler = this.httpClientFactory.CreateHandler();
            using var httpClient = this.httpClientFactory.Create(handler);
            var response = httpClient.Send(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                // 返回 404，已无更多内容，不必自增页数
                return Array.Empty<ArticleModel>();
            }

            var result = Parse(response.Content!.ReadAsStream(default));
            this.page++;
            return result;
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
}