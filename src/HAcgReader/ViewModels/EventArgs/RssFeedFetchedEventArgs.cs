using System;

namespace HAcgReader.ViewModels;

/// <summary>
/// RSS Feed 拉取完毕事件参数
/// </summary>
public class RssFeedFetchedEventArgs : EventArgs
{
    /// <summary>
    /// 总页面个数
    /// </summary>
    public int Total { get; init; }
}
