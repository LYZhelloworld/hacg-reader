using System.Windows;
using HAcgReader.Models;

namespace HAcgReader;

/// <summary>
/// MainWindow.xaml 交互逻辑
/// </summary>
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
}
