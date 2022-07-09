using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace HAcgReader.Models;

/// <summary>
/// <see cref="MainWindow"/> 的模型
/// </summary>
public class MainWindowModel : INotifyPropertyChanged
{
    /// <summary>
    /// 文章列表
    /// </summary>
    private readonly List<ArticleModel> _articles = new()
    {
        new()
        {
            Title = "test",
            Link = "https://example.com",
            CommentLink = "https://example.com",
            MagnetLinks = new string[] { "a", "b" },
            Creator = "creator",
            PubDate = System.DateTime.Now,
            Categories = new string[] { "cat1", "cat2" },
            CommentCount = 50,
        }
    };

    /// <summary>
    /// 文章列表属性
    /// </summary>
    public IEnumerable<ArticleModel> Articles => _articles;

    /// <summary>
    /// 选中的列表项
    /// </summary>
    private int _selectedIndex = -1;

    /// <summary>
    /// 选中的列表项属性
    /// </summary>
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
    /// <summary>
    /// 是否显示详情页
    /// </summary>
    public bool ShowDetails => SelectedIndex >= 0 && SelectedIndex < _articles.Count;

    /// <summary>
    /// 详情页可见性
    /// </summary>
    public Visibility DetailVisibility => ShowDetails ? Visibility.Visible : Visibility.Hidden;

    /// <summary>
    /// 选中的文章
    /// </summary>
    /// <remarks>
    /// 如果没有选中项，返回空白文章。
    /// </remarks>
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

    /// <summary>
    /// 空文章，仅做占位符用
    /// </summary>
    private static readonly ArticleModel s_emptyArticle = new();

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
}
