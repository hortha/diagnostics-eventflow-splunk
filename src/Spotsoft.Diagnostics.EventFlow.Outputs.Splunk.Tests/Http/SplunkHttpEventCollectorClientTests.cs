// ------------------------------------------------------------------------------------------------
//  Copyright (c) Andrew Horth.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.EventFlow;
using Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Configuration;
using Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Http;
using Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Tests.TestHelpers;
using Xunit;

namespace Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Tests.Http
{
    public class SplunkHttpEventCollectorClientTests
    {
        [Fact]
        public async Task SendEventsAsync_WithNonEmptyEvents_CallsHttpEventCollectorApi()
        {
            // Arrange            
            var splunkOutputConfiguration = new SplunkOutputConfiguration()
            {
                ServiceBaseAddress = "https://hec.mysplunkserver.com:8088",
                AuthenticationToken = "B5A79AAD-D822-46CC-80D1-819F80D7BFB0",
                Host = "MY-COMPUTER"
            };
            var uriExpected = new Uri("https://hec.mysplunkserver.com:8088/services/collector/event/1.0");
            var fakeResponseHandler = new FakeResponseHandler();
            fakeResponseHandler.AddFakeResponse(
                uriExpected,
                new HttpResponseMessage(HttpStatusCode.OK));
            var httpClient = new HttpClient(fakeResponseHandler);
            var events = new ReadOnlyCollection<EventData>(new List<EventData>()
            {
                new EventData() { Level = LogLevel.Error, ProviderName = "MyNamespace.MyClass", Payload =
                {
                    new KeyValuePair<string, object>("MyKey", "MyValue")
                }} 
            });
            var cancellationToken = new CancellationToken();

            // Act
            var splunkHttpEventCollectorClient = new SplunkHttpEventCollectorClient(httpClient, splunkOutputConfiguration);
            await splunkHttpEventCollectorClient.SendEventsAsync(events, cancellationToken);

            // Assert
            Assert.Equal(1, fakeResponseHandler.GetResponseCallCount(uriExpected));
        }

        [Fact]
        public async Task SendEventsAsync_WithServiceBaseAddressWithPartialPath_CallsHttpEventCollectorApi()
        {
            // Arrange            
            var splunkOutputConfiguration = new SplunkOutputConfiguration()
            {
                ServiceBaseAddress = "https://hec.mysplunkserver.com:8088/api/",
                AuthenticationToken = "B5A79AAD-D822-46CC-80D1-819F80D7BFB0",
                Host = "MY-COMPUTER"
            };
            var uriExpected = new Uri("https://hec.mysplunkserver.com:8088/api/services/collector/event/1.0");            
            var fakeResponseHandler = new FakeResponseHandler();
            fakeResponseHandler.AddFakeResponse(
                uriExpected,
                new HttpResponseMessage(HttpStatusCode.OK));
            var httpClient = new HttpClient(fakeResponseHandler);
            var events = new ReadOnlyCollection<EventData>(new List<EventData>()
            {
                new EventData()
                {
                    Level = LogLevel.Error,
                    ProviderName = "MyNamespace.MyClass",
                    Payload =
                    {
                        new KeyValuePair<string, object>("MyKey", "MyValue")
                    }
                }
            });
            var cancellationToken = new CancellationToken();

            // Act
            var splunkHttpEventCollectorClient = new SplunkHttpEventCollectorClient(httpClient, splunkOutputConfiguration);
            await splunkHttpEventCollectorClient.SendEventsAsync(events, cancellationToken);

            // Assert
            Assert.Equal(1, fakeResponseHandler.GetResponseCallCount(uriExpected));
        }

        [Fact]
        public async Task SendEventsAsync_WhenApiReturnsErrorResponse_Throws()
        {
            // Arrange            
            var splunkOutputConfiguration = new SplunkOutputConfiguration()
            {
                ServiceBaseAddress = "https://hec.mysplunkserver.com:8088",
                AuthenticationToken = "B5A79AAD-D822-46CC-80D1-819F80D7BFB0",
                Host = "MY-COMPUTER"
            };
            var uriExpected = new Uri("https://hec.mysplunkserver.com:8088/services/collector/event/1.0");
            var fakeResponseHandler = new FakeResponseHandler();
            fakeResponseHandler.AddFakeResponse(
                uriExpected,
                new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    ReasonPhrase = "Service Unavailable",
                    Content =
                        new StringContent(
                            "Something bad happened",
                            Encoding.UTF8,
                            "application/json")
                });
            var httpClient = new HttpClient(fakeResponseHandler);
            var events = new ReadOnlyCollection<EventData>(new List<EventData>()
            {
                new EventData() { Level = LogLevel.Error, ProviderName = "MyNamespace.MyClass", Payload =
                {
                    new KeyValuePair<string, object>("MyKey", "MyValue")
                }}
            });
            var cancellationToken = new CancellationToken();

            // Act
            var splunkHttpEventCollectorClient = new SplunkHttpEventCollectorClient(httpClient, splunkOutputConfiguration);
            await Assert.ThrowsAnyAsync<Exception>(() => splunkHttpEventCollectorClient.SendEventsAsync(events, cancellationToken));

            // Assert
            Assert.Equal(1, fakeResponseHandler.GetResponseCallCount(uriExpected));
        }
    }
}