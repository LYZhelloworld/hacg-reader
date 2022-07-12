using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
        ArticleListViewModel.ArticleSelected += ArticleListViewModel_ArticleSelected;
        ArticleListViewModel.FetchStarted += ArticleListViewModel_FetchStarted;
        ArticleListViewModel.RssFeedFetched += ArticleListViewModel_RssFeedFetched;
        ArticleListViewModel.PageAnalysisCompleted += ArticleListViewModel_PageAnalysisCompleted;
        ArticleListViewModel.FetchCompleted += ArticleListViewModel_FetchCompleted;
        FetchButtonViewModel.Clicked += FetchButtonViewModel_Clicked;
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// <see cref="ArticleListViewModel.ArticleSelected"/> 事件处理
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void ArticleListViewModel_ArticleSelected(object? sender, ArticleSelectedEventArgs e)
    {
        DetailPageViewModel.SelectedArticle = e.SelectedArticle;
        DetailPageViewModel.Visibility = Visibility.Visible;
    }

    /// <summary>
    /// <see cref="ArticleListViewModel.FetchStarted"/> 事件处理
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void ArticleListViewModel_FetchStarted(object? sender, System.EventArgs e)
    {
        ProgressBarViewModel.IsIndeterminate = true;
    }

    /// <summary>
    /// <see cref="ArticleListViewModel.RssFeedFetched"/> 事件处理
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void ArticleListViewModel_RssFeedFetched(object? sender, RssFeedFetchedEventArgs e)
    {
        ProgressBarViewModel.IsIndeterminate = false;
        ProgressBarViewModel.Maximum = e.Total;
        ProgressBarViewModel.Value = 0;
    }

    /// <summary>
    /// <see cref="ArticleListViewModel.PageAnalysisCompleted"/> 事件处理
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void ArticleListViewModel_PageAnalysisCompleted(object? sender, PageAnalysisCompletedEventArgs e)
    {
        ProgressBarViewModel.Value = e.Progress;
    }

    /// <summary>
    /// <see cref="ArticleListViewModel.FetchCompleted"/> 事件处理
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void ArticleListViewModel_FetchCompleted(object? sender, System.EventArgs e)
    {
        ProgressBarViewModel.Value = 0;
        FetchButtonViewModel.IsEnabled = true;
    }

    /// <summary>
    /// <see cref="FetchButtonViewModel.Clicked"/> 事件处理
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void FetchButtonViewModel_Clicked(object? sender, System.EventArgs e)
    {
        FetchButtonViewModel.IsEnabled = false;
        Task.Run(ArticleListViewModel.FetchAsync);
    }
    #endregion
}