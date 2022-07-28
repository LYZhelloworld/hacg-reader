// <copyright file="IRssFeedService.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.Services
{
    using HAcgReader.Models;

    /// <summary>
    /// 获取神社 RSS Feed
    /// </summary>
    public interface IRssFeedService
    {
        /// <summary>
        /// 获取下一页 RSS Feed 内容
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>获取的下一页内容</returns>
        /// <exception cref="TaskCanceledException">在任务取消时抛出</exception>
        IEnumerable<ArticleModel> FetchNext(CancellationToken cancellationToken);
    }
}