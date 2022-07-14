using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HAcgReader.ViewModels;

/// <summary>
/// 主窗口视图模型
/// </summary>
public class MainViewModel : BaseViewModel
{
    #region Fields
    /// <summary>
    /// 文章列表视图模型
    /// </summary>
    public ArticleListViewModel ArticleListViewModel { get; private set; }

    /// <summary>
    /// 拉取按钮视图模型
    /// </summary>
    public FetchButtonViewModel FetchButtonViewModel { get; private set; }

    /// <summary>
    /// 详情页视图模型
    /// </summary>
    public DetailPageViewModel DetailPageViewModel { get; private set; }

    /// <summary>
    /// 滚动条视图模型
    /// </summary>
    public ProgressBarViewModel ProgressBarViewModel { get; private set; }

    /// <summary>
    /// 拉取事件 <see cref="CancellationToken"/> 源
    /// </summary>
    private CancellationTokenSource? _fetchingCancellationTokenSource;

    /// <summary>
    /// 拉取事件
    /// </summary>
    private Task? _fetchingTask;
    #endregion

    #region Constructors
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="articleListViewModel">文章列表视图模型</param>
    /// <param name="fetchButtonViewModel">拉取按钮视图模型</param>
    /// <param name="detailPageViewModel">详情页视图模型</param>
    /// <param name="progressBarViewModel">滚动条视图模型</param>
    public MainViewModel(ArticleListViewModel articleListViewModel,
                         FetchButtonViewModel fetchButtonViewModel,
                         DetailPageViewModel detailPageViewModel,
                         ProgressBarViewModel progressBarViewModel)
    {
        ArticleListViewModel = articleListViewModel;
        FetchButtonViewModel = fetchButtonViewModel;
        DetailPageViewModel = detailPageViewModel;
        ProgressBarViewModel = progressBarViewModel;

        InitializeViewModelEventHandlers();
    }
    #endregion

    #region Methods
    /// <summary>
    /// 初始化视图模型事件监听器
    /// </summary>
    private void InitializeViewModelEventHandlers()
    {
        ArticleListViewModel.ArticleSelected += (sender, e) =>
        {
            DetailPageViewModel.SelectedArticle = e.SelectedArticle;
            DetailPageViewModel.Visibility = Visibility.Visible;
        };
        ArticleListViewModel.FetchStarted += (sender, e) =>
        {
            ProgressBarViewModel.IsIndeterminate = true;
        };
        ArticleListViewModel.RssFeedFetched += (sender, e) =>
        {
            ProgressBarViewModel.IsIndeterminate = false;
            ProgressBarViewModel.Maximum = e.Total;
            ProgressBarViewModel.Value = 0;
        };
        ArticleListViewModel.PageAnalysisCompleted += (sender, e) =>
        {
            ProgressBarViewModel.Value = e.Progress;
        };
        ArticleListViewModel.FetchCompleted += (sender, e) =>
        {
            ProgressBarViewModel.Value = 0;
            FetchButtonViewModel.IsFetching = false;

            _fetchingCancellationTokenSource?.Dispose();
            _fetchingCancellationTokenSource = null;
            _fetchingTask = null;
        };
        ArticleListViewModel.FetchCancelled += (sender, e) =>
        {
            ProgressBarViewModel.Value = 0;
            ProgressBarViewModel.IsIndeterminate = false;
        };

        FetchButtonViewModel.Started += (sender, e) =>
        {
            _fetchingCancellationTokenSource = new();
            _fetchingTask = Task.Run(() => ArticleListViewModel.Fetch(_fetchingCancellationTokenSource.Token));
        };
        FetchButtonViewModel.Cancelled += (sender, e) =>
        {
            FetchButtonViewModel.IsEnabled = false;
            _fetchingCancellationTokenSource?.Cancel();
            _fetchingCancellationTokenSource?.Dispose();
            Task.Run(() =>
            {
                _fetchingTask?.Wait();
                _fetchingTask = null;
                FetchButtonViewModel.IsEnabled = true;
            });
        };
    }
    #endregion
}