using HAcgReader.Models;
using HAcgReader.Services;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HAcgReader;

/// <summary>
/// MainWindow.xaml 交互逻辑
/// </summary>
[ExcludeFromCodeCoverage]
public partial class MainWindow : Window
{
    /// <summary>
    /// 模型
    /// </summary>
    private readonly IMainWindowModel _model;

    /// <summary>
    /// 是否正在拉取新的文章
    /// </summary>
    private bool _fetchingNewArticles;

    /// <summary>
    /// 模型属性
    /// </summary>
    public IMainWindowModel Model => _model;

    /// <summary>
    /// 构造函数
    /// </summary>
    public MainWindow()
    {
        var domainService = new DomainService();
        var domain = domainService.GetDomain();
        if (string.IsNullOrEmpty(domain))
        {
            MessageBox.Show("找不到神社域名。请确保 acg.gy 能够正常访问。",
                            "HAcgReader",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
            Close();
        }

        _model = new MainWindowModel(domain);

        InitializeComponent();
        DataContext = _model;
        FetchNewArticles();
    }

    /// <summary>
    /// 拉取新文章
    /// </summary>
    private void FetchNewArticles()
    {
        // 避免重复触发
        if (_fetchingNewArticles)
        {
            return;
        }

        _fetchingNewArticles = true;
        Task.Run(async () =>
        {
            await _model.FetchNewArticlesAsync().ConfigureAwait(false);
            _fetchingNewArticles = false;
        });
    }

    /// <summary>
    /// <see cref="ArticleList"/> 被选择的项目更改时触发，用于更改详情页显示的内容
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void ArticleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _model.SelectedIndex = ArticleList.SelectedIndex;
    }

    /// <summary>
    /// <see cref="ArticleList"/> 滚动时触发，用于拉取新文章
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void ArticleList_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        // 避免重复触发
        if (_fetchingNewArticles)
        {
            return;
        }

        // 向下滚动，且滚动超出范围
        if (e.VerticalChange > 0 && e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight)
        {
            FetchNewArticles();
        }
    }
}
