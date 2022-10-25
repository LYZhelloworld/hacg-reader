// <copyright file="PageAnalyzerService.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.Core.Services
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using HAcgReader.Core.Factories;
    using HAcgReader.Core.Models;
    using HtmlAgilityPack;

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
        private static readonly Regex MagnetLink = new(@"(?<![0-9a-fA-F])([0-9a-fA-F]{40})(?![0-9a-fA-F])", RegexOptions.Compiled);

        /// <summary>
        /// HTTP 客户端工厂类
        /// </summary>
        private readonly IHttpClientFactory httpClientFactory;

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
            this.httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc/>
        public ArticleModel Analyze(ArticleModel article, CancellationToken cancellationToken)
        {
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article));
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, article.Link);
            using var handler = this.httpClientFactory.CreateHandler();
            using var httpClient = this.httpClientFactory.Create(handler);
            var response = httpClient.Send(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                // 无法解析
                return article;
            }

            article.MagnetLinks = Parse(response.Content!.ReadAsStream(default), out var entryContent);
            article.Preview = entryContent;
            return article;
        }

        /// <summary>
        /// 解析返回的内容
        /// </summary>
        /// <param name="content">返回内容</param>
        /// <param name="preview">解析出的预览文本</param>
        /// <returns>解析出的磁链集合</returns>
        private static IEnumerable<string> Parse(Stream content, out string preview)
        {
            var doc = new HtmlDocument();
            doc.Load(content);

            preview = string.Empty;

            // 寻找 <div class="entry-content">
            var entryContentTags = doc.DocumentNode.SelectNodes("//div[@class=\"entry-content\"]");
            if (entryContentTags == null)
            {
                // 没有找到对应标签
                return Array.Empty<string>();
            }

            preview = string.Join(string.Empty, entryContentTags.AsEnumerable()
                .Select(GeneratePreview)).Trim();

            return entryContentTags.AsEnumerable()
                .Select(entryContentTag => entryContentTag.InnerHtml)
                .Select(innerHtml =>
                {
                    var matches = MagnetLink.Matches(innerHtml);
                    return matches.Select(match => match.Groups[1].Value).Distinct();
                })
                .SelectMany(i => i)
                .Select(i => MagnetPrefix + i);
        }

        /// <summary>
        /// 解析正文内容，去除不需要的标签并生成纯文本预览
        /// </summary>
        /// <param name="tag">当前 HTML 标签</param>
        /// <returns>预览文本</returns>
        private static string GeneratePreview(HtmlNode tag)
        {
            return tag.NodeType switch
            {
                HtmlNodeType.Text => tag.InnerText.Trim(),
                HtmlNodeType.Element => tag.Name switch
                {
                    "p" or "div" =>
                        Environment.NewLine +
                        string.Join(string.Empty, tag.ChildNodes.AsEnumerable().Select(GeneratePreview)).Trim() +
                        Environment.NewLine,
                    "span" => string.Join(string.Empty, tag.ChildNodes.AsEnumerable().Select(GeneratePreview)).Trim(),
                    "br" => Environment.NewLine,
                    _ => string.Empty,
                },
                _ => string.Empty,
            };
        }
    }
}
