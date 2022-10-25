// <copyright file="IDomainService.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.Core.Services
{
    /// <summary>
    /// 提供与神社域名相关的功能
    /// </summary>
    public interface IDomainService
    {
        /// <summary>
        /// 获取神社使用的域名
        /// </summary>
        /// <returns>使用的域名，如果获取失败则返回空字符串</returns>
        string GetDomain();
    }
}
