namespace HAcgReader.ViewModels;

/// <summary>
/// 滚动条视图模型
/// </summary>
public class ProgressBarViewModel : BaseViewModel
{
    #region View Model Properties
    /// <summary>
    /// 滚动条当前值
    /// </summary>
    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 滚动条当前值
    /// </summary>
    private int _value;

    /// <summary>
    /// 滚动条最大值
    /// </summary>
    public int Maximum
    {
        get => _maximum;
        set
        {
            _maximum = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 滚动条最大值
    /// </summary>
    private int _maximum = 10;

    /// <summary>
    /// 滚动条处于不确定状态
    /// </summary>
    public bool IsIndeterminate
    {
        get => _IsIndeterminate;
        set
        {
            _IsIndeterminate = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 滚动条处于不确定状态
    /// </summary>
    private bool _IsIndeterminate;
    #endregion
}
