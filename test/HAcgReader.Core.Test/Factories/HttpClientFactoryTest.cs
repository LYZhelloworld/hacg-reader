﻿// <copyright file="HttpClientFactoryTest.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.Core.Test.Factories
{
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using HAcgReader.Core.Factories;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// 测试 <see cref="HttpClientFactory"/>
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class HttpClientFactoryTest
    {
        /// <summary>
        /// 测试 <see cref="HttpClientFactory.Create"/>
        /// </summary>
        [TestMethod]
        public void TestCreate()
        {
            var factory = new HttpClientFactory();
            using var httpClient = factory.Create();
            httpClient.Should().NotBeNull();

            using var handler = factory.CreateHandler();
            using var httpClient2 = factory.Create(handler);
            httpClient2.Should().NotBeNull();
        }
    }
}
