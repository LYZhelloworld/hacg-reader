using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HAcgReader.ViewModels;

/// <summary>
/// 视图模型基类
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 属性改变事件处理
    /// </summary>
    /// <param name="propertyName">属性名</param>
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }
}
