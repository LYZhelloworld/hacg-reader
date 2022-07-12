using HAcgReader.Resources;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Windows.Input;

namespace HAcgReader.ViewModels;

/// <summary>
/// 拉取按钮视图模型
/// </summary>
public class FetchButtonViewModel : BaseViewModel
{
    #region View Model Properties
    /// <summary>
    /// 拉取按钮是否有效
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ButtonText));
            _command.NotifyCanExecuteChanged();
        }
    }

    /// <summary>
    /// 拉取按钮是否有效
    /// </summary>
    private bool _isEnabled = true;

    /// <summary>
    /// 按钮文本
    /// </summary>
    public string ButtonText => IsEnabled ? Strings.FetchButtonText : Strings.FetchButtonTextDisabled;
    #endregion

    #region Commands
    /// <summary>
    /// 拉取命令
    /// </summary>
    public ICommand Command => _command;

    /// <summary>
    /// 拉取命令
    /// </summary>
    private readonly RelayCommand _command;
    #endregion

    #region Events
    /// <summary>
    /// 按钮点击事件
    /// </summary>
    public event EventHandler? Clicked;
    #endregion

    #region Constructors
    /// <summary>
    /// 构造函数
    /// </summary>
    public FetchButtonViewModel()
    {
        _command = new RelayCommand(Execute, () => IsEnabled);
    }
    #endregion

    #region Methods
    /// <summary>
    /// 按钮点击
    /// </summary>
    public void Execute()
    {
        if (IsEnabled)
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }
    }
    #endregion
}
