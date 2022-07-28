// <copyright file="RssFeedFetchedEventArgs.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.ViewModels
{
    using System;

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
}