using HAcgReader.Services;
using HAcgReader.ViewModels;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;

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
        ViewModel.StartedArticleProcessing += ViewModel_StartedArticleProcessing;
        ViewModel.ArticleProcessed += ViewModel_ArticleProcessed;
        ViewModel.AllArticlesProcessed += ViewModel_AllArticlesProcessed;

        InitializeComponent();
        DataContext = ViewModel;
        ViewModel.Fetch();
    }

    private void ViewModel_StartedArticleProcessing(object? sender, StartedArticleProcessingEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            Progress.Maximum = e.Total;
            CommandManager.InvalidateRequerySuggested();
        });
    }

    private void ViewModel_ArticleProcessed(object? sender, ArticleProcessedEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            Progress.Value = e.Processed;
            CommandManager.InvalidateRequerySuggested();
        });
    }

    private void ViewModel_AllArticlesProcessed(object? sender, System.EventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            Progress.Value = 0;
            CommandManager.InvalidateRequerySuggested();
        });
    }
}
