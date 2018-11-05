// ------------------------------------------------------------------------------------------------
//  Copyright (c) Andrew Horth.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Net.Http;
using Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Configuration;

namespace Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Http
{
    internal class SplunkHttpMessageHandlerFactory : ISplunkHttpMessageHandlerFactory
    {
        private readonly int maxRetryAttempts;
        private readonly bool ignoreSslCertificateErrors;

        public SplunkHttpMessageHandlerFactory(SplunkOutputConfiguration configuration)
        {
            maxRetryAttempts = configuration.MaxRetryAttempts;
            ignoreSslCertificateErrors = configuration.IgnoreSslCertificateErrors;
        }

        public HttpMessageHandler Create()
        {
            HttpMessageHandler httpMessageHandler = null;
#if NETSTANDARD
            var innerHandler = new HttpClientHandler();
            if (ignoreSslCertificateErrors)
            {
                innerHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }
            
            httpMessageHandler = new HttpExponentialRetryMessageHandler(innerHandler, maxRetryAttempts);
#else
            var innerHandler = new WebRequestHandler();
            if (ignoreSslCertificateErrors)
            {
                innerHandler.ServerCertificateValidationCallback = (message, cert, chain, errors) => true;
            }

            httpMessageHandler = new HttpExponentialRetryMessageHandler(innerHandler, maxRetryAttempts);
#endif
            return httpMessageHandler;
        }
    }
}