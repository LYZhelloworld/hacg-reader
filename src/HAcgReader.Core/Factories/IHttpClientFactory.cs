// <copyright file="IHttpClientFactory.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.Factories
{
    /// <summary>
    /// HTTP 客户端工厂类
    /// </summary>
    public interface IHttpClientFactory
    {
        /// <summary>
        /// 创造 <see cref="HttpClient"/> 实例
        /// </summary>
        /// <returns><see cref="HttpClient"/> 对象</returns>
        HttpClient Create();
    }
}