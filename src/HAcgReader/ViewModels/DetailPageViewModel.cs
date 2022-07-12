using HAcgReader.Models;
using System.Windows;

namespace HAcgReader.ViewModels;

/// <summary>
/// 详情页视图模型
/// </summary>
public class DetailPageViewModel : BaseViewModel
{
    #region View Model Properties
    /// <summary>
    /// 被选中的文章
    /// </summary>
    public ArticleModel SelectedArticle
    {
        get => _selectedArticle;
        set
        {
            _selectedArticle = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 被选中的文章
    /// </summary>
    private ArticleModel _selectedArticle = new();

    /// <summary>
    /// 详情页可见性
    /// </summary>
    public Visibility Visibility
    {
        get => _Visibility;
        set
        {
            _Visibility = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 详情页可见性
    /// </summary>
    private Visibility _Visibility = Visibility.Hidden;
    #endregion
}
