// ------------------------------------------------------------------------------------------------
//  Copyright (c) Andrew Horth.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.EventFlow;
using Newtonsoft.Json;
using Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Configuration;

namespace Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Http
{
    internal class SplunkHttpEventCollectorClient : ISplunkHttpEventCollectorClient
    {
        private const string JsonMediaType = "application/json";
        private const string HttpEventCollectorResource = "services/collector/event/1.0"; // Note no leading backslash as caused problems if base address had a partial path in it (e.g. https://mydomain.com/splunk/)
        private const string AuthorizationHeaderScheme = "Splunk";
        private readonly HttpClient httpClient;
        private readonly SplunkOutputConfiguration configuration;

        public SplunkHttpEventCollectorClient(
            HttpClient httpClient, 
            SplunkOutputConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationHeaderScheme, configuration.AuthenticationToken);
            httpClient.BaseAddress = new Uri(configuration.ServiceBaseAddress, UriKind.Absolute);
        }

        public async Task SendEventsAsync(IReadOnlyCollection<EventData> events, CancellationToken cancellationToken)
        {
            if (events == null || events.Count == 0)
            {
                return;
            }

            var serializedEvents = new StringBuilder();
            foreach (var eventData in events)
            {
                string jsonData = JsonConvert.SerializeObject(
                    new SplunkEventData(eventData, configuration.Host, configuration.Index, configuration.Source, configuration.SourceType));
                serializedEvents.Append(jsonData);
            }

            HttpContent content = new StringContent(serializedEvents.ToString(), Encoding.UTF8, JsonMediaType);
            HttpResponseMessage response = await httpClient.PostAsync(HttpEventCollectorResource, content, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                string responseContent = string.Empty;
                try
                {
                    responseContent = await response.Content.ReadAsStringAsync();
                }
                catch
                {
                    // Swallow exception trying to get the response content - don't want this to mask the original problem!
                }

                string errorMessage = $"{nameof(SplunkHttpEventCollectorClient)}: Splunk HTTP Event Collector REST API returned an error. Code: {response.StatusCode} Description: {response.ReasonPhrase} {responseContent}";
                throw new Exception(errorMessage);
            }
        }
    }
}