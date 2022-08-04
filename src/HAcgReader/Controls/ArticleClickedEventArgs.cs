// <copyright file="ArticleClickedEventArgs.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.Controls
{
    using System;
    using HAcgReader.Core.Models;

    /// <summary>
    /// 列表项点击事件参数
    /// </summary>
    public class ArticleClickedEventArgs : EventArgs
    {
        /// <summary>
        /// 被点击的文章
        /// </summary>
        public ArticleModel Article { get; set; } = new();
    }
}
