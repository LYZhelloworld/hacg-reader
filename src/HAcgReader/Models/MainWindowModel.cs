using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace HAcgReader.Models;

/// <summary>
/// <see cref="IMainWindowModel"/> 的实现
/// </summary>
public class MainWindowModel : IMainWindowModel
{
    /// <summary>
    /// 空文章，仅做占位符用
    /// </summary>
    private static readonly ArticleModel s_emptyArticle = new();

    /// <summary>
    /// 文章列表
    /// </summary>
    private readonly List<ArticleModel> _articles = new();

    /// <inheritdoc/>
    public IEnumerable<ArticleModel> Articles => _articles;

    /// <summary>
    /// 选中的列表项
    /// </summary>
    private int _selectedIndex = -1;

    /// <inheritdoc/>
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            _selectedIndex = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DetailVisibility));
            OnPropertyChanged(nameof(SelectedArticle));
        }
    }

    /// <inheritdoc/>
    public bool ShowDetails => SelectedIndex >= 0 && SelectedIndex < _articles.Count;

    /// <inheritdoc/>
    public Visibility DetailVisibility => ShowDetails ? Visibility.Visible : Visibility.Hidden;

    /// <inheritdoc/>
    public ArticleModel SelectedArticle
    {
        get
        {
            if (ShowDetails)
            {
                return _articles[SelectedIndex];
            }
            else
            {
                return s_emptyArticle;
            }
        }
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 构造函数
    /// </summary>
    public MainWindowModel()
    {
    }

    /// <summary>
    /// 属性改变事件处理
    /// </summary>
    /// <param name="propertyName">属性名</param>
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    /// <inheritdoc/>
    public void AddArticles(params ArticleModel[] articles)
    {
        _articles.AddRange(articles);
        OnPropertyChanged(nameof(Articles));
    }
}
