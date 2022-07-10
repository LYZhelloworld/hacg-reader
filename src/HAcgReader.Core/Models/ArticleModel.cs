namespace HAcgReader.Models;

/// <summary>
/// 文章模型
/// </summary>
public record ArticleModel
{
    /// <summary>
    /// 文章标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 文章链接
    /// </summary>
    public string Link { get; set; } = string.Empty;

    /// <summary>
    /// 评论链接
    /// </summary>
    public string CommentLink { get; set; } = string.Empty;

    /// <summary>
    /// 解析出的磁力链接
    /// </summary>
    public IEnumerable<string> MagnetLinks { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// 作者名
    /// </summary>
    public string Creator { get; set; } = string.Empty;

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime PubDate { get; set; }

    /// <summary>
    /// 分类
    /// </summary>
    public IEnumerable<string> Categories { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// 评论数量
    /// </summary>
    public int CommentCount { get; set; }
}
