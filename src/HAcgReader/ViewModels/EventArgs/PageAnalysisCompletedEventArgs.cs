using System;

namespace HAcgReader.ViewModels;

/// <summary>
/// 页面分析结束事件参数
/// </summary>
public class PageAnalysisCompletedEventArgs : EventArgs
{
    /// <summary>
    /// 已处理的页面数量
    /// </summary>
    public int Progress { get; init; }
}
