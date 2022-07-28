// <copyright file="PageAnalysisCompletedEventArgs.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.ViewModels
{
    using System;

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
}