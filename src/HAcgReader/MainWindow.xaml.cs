using HAcgReader.Resources;
using HAcgReader.Services;
using HAcgReader.ViewModels;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace HAcgReader;

/// <summary>
/// MainWindow.xaml 交互逻辑
/// </summary>
public partial class MainWindow : Window
{
    #region Fields
    /// <summary>
    /// 主窗口视图模型
    /// </summary>
    public MainViewModel MainViewModel { get; private set; }
    #endregion

    #region Constructors
    /// <summary>
    /// 构造函数
    /// </summary>
    public MainWindow()
    {
        var domainService = new DomainService();
        var domain = domainService.GetDomain();
        if (string.IsNullOrEmpty(domain))
        {
            MessageBox.Show(Strings.ErrorCannotRetrieveDomain,
                            Strings.Title,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
            Close();
        }

        MainViewModel = new MainViewModel(articleListViewModel: new ArticleListViewModel(domain),
                                          fetchButtonViewModel: new FetchButtonViewModel(),
                                          detailPageViewModel: new DetailPageViewModel(),
                                          progressBarViewModel: new ProgressBarViewModel());

        InitializeComponent();
        MainViewModel.FetchButtonViewModel.Command.Execute(null);
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// 文章链接、文章评论链接点击事件处理
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void Link_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        var link = ((Hyperlink)sender).NavigateUri.AbsoluteUri;
        Process.Start(new ProcessStartInfo()
        {
            FileName = link,
            UseShellExecute = true,
        });
        e.Handled = true;
    }

    /// <summary>
    /// 磁力链接点击事件处理
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void MagnetLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        var magnetLink = ((Hyperlink)sender).NavigateUri.AbsoluteUri;
        Clipboard.SetText(magnetLink);
        MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Strings.MagnetLinkCopied, magnetLink), Strings.Title, MessageBoxButton.OK, MessageBoxImage.Information);
    }
    #endregion
}
