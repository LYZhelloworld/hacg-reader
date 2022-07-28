// <copyright file="HttpClientFactory.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.Factories
{
    /// <summary>
    /// HTTP 客户端工厂类
    /// </summary>
    public class HttpClientFactory : IHttpClientFactory
    {
        /// <inheritdoc/>
        public HttpClient Create()
        {
            return new HttpClient();
        }
    }
}