using HAcgReader.Services;
using HAcgReader.ViewModels;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;

namespace HAcgReader;

/// <summary>
/// MainWindow.xaml 交互逻辑
/// </summary>
[ExcludeFromCodeCoverage]
public partial class MainWindow : Window
{
    /// <summary>
    /// 模型属性
    /// </summary>
    public MainWindowViewModel ViewModel { get; private set; }

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

        ViewModel = new MainWindowViewModel(domain);
        ViewModel.FetchCompleted += ViewModel_FetchCompleted;

        InitializeComponent();
        DataContext = ViewModel;
        ViewModel.FetchCommand.Execute(null);
    }

    /// <summary>
    /// 拉取完毕事件处理
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void ViewModel_FetchCompleted(object? sender, EventArgs e)
    {
        Dispatcher.Invoke(() => CommandManager.InvalidateRequerySuggested());
    }

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
        MessageBox.Show($"已复制磁链：\n{magnetLink}", "HAcgReader", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
