using HAcgReader.Models;

namespace HAcgReader.Services;

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