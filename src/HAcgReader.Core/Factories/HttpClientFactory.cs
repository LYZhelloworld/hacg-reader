// <copyright file="HttpClientFactory.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.Core.Factories
{
    using System.Net;

    /// <summary>
    /// HTTP 客户端工厂类
    /// </summary>
    public class HttpClientFactory : IHttpClientFactory
    {
        /// <summary>
        /// 默认 User Agent
        /// </summary>
        private const string UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/103.0.5060.66 Safari/537.36 Edg/103.0.1264.44";

        /// <inheritdoc/>
        public HttpClientHandler CreateHandler(bool useProxy = true)
        {
            return new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                UseProxy = useProxy,
            };
        }

        /// <inheritdoc/>
        public HttpClient Create(HttpClientHandler? handler = null)
        {
            HttpClient client;
            if (handler == null)
            {
                client = new HttpClient();
            }
            else
            {
                client = new HttpClient(handler);
            }

            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            client.DefaultRequestHeaders.AcceptCharset.Add(new("utf-8"));

            return client;
        }
    }
}
