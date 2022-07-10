using HAcgReader.Models;
using HAcgReader.Services;
using System.Diagnostics.CodeAnalysis;
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
    /// 模型属性
    /// </summary>
    public IMainWindowModel Model => _model;

    /// <summary>
    /// 构造函数
    /// </summary>
    public MainWindow()
    {
        using var domainService = new DomainService();
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
    }

    /// <summary>
    /// <see cref="ArticleList"/> 被选择的项目更改时触发
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void ArticleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _model.SelectedIndex = ArticleList.SelectedIndex;
    }
}
