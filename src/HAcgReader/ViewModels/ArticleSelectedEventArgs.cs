// <copyright file="ArticleSelectedEventArgs.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.ViewModels
{
    using System;
    using HAcgReader.Core.Models;

    /// <summary>
    /// 选中文章事件参数
    /// </summary>
    public class ArticleSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// 被选中文章
        /// </summary>
        public ArticleModel SelectedArticle { get; init; } = new();
    }
}