using HAcgReader.Models;
using System;

namespace HAcgReader.ViewModels;

/// <summary>
/// 选中文章事件参数
/// </summary>
public class ArticleSelectedEventArgs : EventArgs
{
    /// <summary>
    /// 被选中文章
    /// </summary>
    public ArticleModel SelectedArticle { get; init; } = new();
}
