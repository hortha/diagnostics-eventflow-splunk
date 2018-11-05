// ------------------------------------------------------------------------------------------------
//  Copyright (c) Andrew Horth.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Net.Http;
using Moq;
using Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Http;
using Xunit;

namespace Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Tests.Http
{
    public class SplunkHttpClientFactoryTests
    {
        [Fact]
        public void Create_WhenCalled_CreatesHttpClientWithHandlerFromFactory()
        {
            // Arrange
            var httpMessageHandlerFactory = new Mock<ISplunkHttpMessageHandlerFactory>();
            var httpMessageHandler = new Mock<HttpMessageHandler>();

            httpMessageHandlerFactory
                .Setup(f => f.Create())
                .Returns(httpMessageHandler.Object)
                .Verifiable();

            // Act
            var httpClientFactory = new SplunkHttpClientFactory(httpMessageHandlerFactory.Object);
            var httpClient = httpClientFactory.Create();

            // Assert
            Assert.NotNull(httpClient);
            httpMessageHandlerFactory.Verify();
        }
    }
}