// <copyright file="IHttpClientFactory.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.Core.Factories
{
    /// <summary>
    /// HTTP 客户端工厂类
    /// </summary>
    public interface IHttpClientFactory
    {
        /// <summary>
        /// 创造 <see cref="HttpClient"/> 实例
        /// </summary>
        /// <param name="handler">HTTP 客户端处理器，为 <see langword="null"/> 时表示不使用</param>
        /// <returns><see cref="HttpClient"/> 对象</returns>
        HttpClient Create(HttpClientHandler? handler = null);

        /// <summary>
        /// 创造 <see cref="HttpClientHandler"/> 实例
        /// </summary>
        /// <param name="useProxy">是否使用代理</param>
        /// <returns><see cref="HttpClientHandler"/> 对象</returns>
        HttpClientHandler CreateHandler(bool useProxy = true);
    }
}