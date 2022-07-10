using HAcgReader.Services;
using HAcgReader.ViewModels;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

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

        ViewModel = new MainWindowViewModel(new RssFeedService(domain), new PageAnalyzerService());

        InitializeComponent();
        DataContext = ViewModel;
        ViewModel.Fetch();
    }
}
