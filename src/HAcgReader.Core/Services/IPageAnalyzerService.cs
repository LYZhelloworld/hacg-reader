// <copyright file="IPageAnalyzerService.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.Core.Services
{
    using HAcgReader.Core.Models;

    /// <summary>
    /// 分析页面内容，寻找磁链
    /// </summary>
    public interface IPageAnalyzerService
    {
        /// <summary>
        /// 分析页面内容
        /// </summary>
        /// <param name="article">要分析的文章</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>分析结果</returns>
        /// <exception cref="ArgumentNullException"><paramref name="article"/> 为 <c>null</c> 时抛出</exception>
        /// <exception cref="TaskCanceledException">在任务取消时抛出</exception>
        /// <remarks>
        /// 该方法将读取 <see cref="ArticleModel.Link"/> 这个字段并试图获取网页内容。
        /// 结果会保存在 <see cref="ArticleModel.MagnetLinks"/> 这个字段中。
        /// </remarks>
        ArticleModel Analyze(ArticleModel article, CancellationToken cancellationToken);
    }
}