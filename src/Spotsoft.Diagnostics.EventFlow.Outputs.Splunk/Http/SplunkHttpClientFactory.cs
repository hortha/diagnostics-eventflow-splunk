// ------------------------------------------------------------------------------------------------
//  Copyright (c) Andrew Horth.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Net.Http;

namespace Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Http
{
    internal class SplunkHttpClientFactory : ISplunkHttpClientFactory
    {
        private readonly ISplunkHttpMessageHandlerFactory splunkHttpMessageHandlerFactory;

        public SplunkHttpClientFactory(ISplunkHttpMessageHandlerFactory splunkHttpMessageHandlerFactory)
        {
            this.splunkHttpMessageHandlerFactory = splunkHttpMessageHandlerFactory;
        }

        public HttpClient Create()
        {
            var handler = splunkHttpMessageHandlerFactory.Create();
            return new HttpClient(handler);
        }
    }
}