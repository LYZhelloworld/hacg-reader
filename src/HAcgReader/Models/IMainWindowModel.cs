using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HAcgReader.Models;

/// <summary>
/// <see cref="MainWindow"/> 的模型
/// </summary>
public interface IMainWindowModel : INotifyPropertyChanged
{
    /// <summary>
    /// 文章列表属性
    /// </summary>
    IEnumerable<ArticleModel> Articles { get; }

    /// <summary>
    /// 选中的文章
    /// </summary>
    /// <remarks>
    /// 如果没有选中项，返回空白文章。
    /// </remarks>
    ArticleModel SelectedArticle { get; }

    /// <summary>
    /// 选中的列表项属性
    /// </summary>
    int SelectedIndex { get; set; }

    /// <summary>
    /// 是否显示详情页
    /// </summary>
    bool ShowDetails { get; }

    /// <summary>
    /// 添加文章
    /// </summary>
    /// <param name="articles">要添加的文章</param>
    void AddArticles(params ArticleModel[] articles);

    /// <summary>
    /// 异步拉取最新的文章
    /// </summary>
    /// <returns>该任务</returns>
    Task FetchNewArticlesAsync();
}