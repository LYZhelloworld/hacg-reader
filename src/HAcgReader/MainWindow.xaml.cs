using HAcgReader.Models;
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
    private readonly MainWindowModel _model;

    /// <summary>
    /// 模型属性
    /// </summary>
    public MainWindowModel View => _model;

    /// <summary>
    /// 构造函数
    /// </summary>
    public MainWindow()
    {
        _model = new MainWindowModel();

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
