using HAcgReader.Models;

namespace HAcgReader.Services;

/// <summary>
/// 分析页面内容，寻找磁链
/// </summary>
public interface IPageAnalyzerService
{
    /// <summary>
    /// 异步分析页面内容
    /// </summary>
    /// <param name="article">要分析的文章</param>
    /// <returns>分析结果的异步返回值</returns>
    /// <exception cref="ArgumentNullException"><paramref name="article"/> 为 <c>null</c> 时抛出</exception>
    /// <remarks>
    /// 该方法将读取 <see cref="ArticleModel.Link"/> 这个字段并试图获取网页内容。
    /// 结果会保存在 <see cref="ArticleModel.MagnetLinks"/> 这个字段中。
    /// </remarks>
    Task<ArticleModel> AnalyzeAsync(ArticleModel article);
}