// ------------------------------------------------------------------------------------------------
//  Copyright (c) Andrew Horth.  All rights reserved.
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Exponential backoff code based on article at https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/explore-custom-http-call-retries-exponential-backoff
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Http
{
    /// <summary>
    /// Implements a message handler that employs exponential backoff retry policy for HTTP requests
    /// </summary>
    internal class HttpExponentialRetryMessageHandler : DelegatingHandler
    {
        private struct ExponentialBackoff
        {
            private readonly int maxRetryAttempts;
            private readonly int delayMilliseconds;
            private readonly int maxDelayMilliseconds;
            private int retries;
            private int pow;

            public ExponentialBackoff(
                int maxRetryAttempts, 
                int delayMilliseconds,
                int maxDelayMilliseconds)
            {
                this.maxRetryAttempts = maxRetryAttempts;
                this.delayMilliseconds = delayMilliseconds;
                this.maxDelayMilliseconds = maxDelayMilliseconds;
                this.retries = 0;
                this.pow = 1;
            }
            public Task Delay()
            {
                if (retries == maxRetryAttempts)
                {
                    throw new TimeoutException("Max retry attempts exceeded.");
                }

                ++retries;
                if (retries < 31)
                {
                    pow = pow << 1; // m_pow = Pow(2, m_retries - 1)
                }

                int delay = Math.Min(delayMilliseconds * (pow - 1) / 2, maxDelayMilliseconds);
                return Task.Delay(delay);
            }
        }

        private const int DelayMilliseconds = 200;
        private const int MaxDelayMilliseconds = 2000;
        private readonly int maxRetryAttempts;
        private ExponentialBackoff backoff;

        public HttpExponentialRetryMessageHandler(HttpMessageHandler innerHandler, int maxRetryAttempts = 3) 
            : base(innerHandler)
        {
            this.maxRetryAttempts = maxRetryAttempts;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            backoff = new ExponentialBackoff(
                maxRetryAttempts,
                DelayMilliseconds,
                MaxDelayMilliseconds);

            while (true)
            {
                var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                if ((int)response.StatusCode < 500)
                {
                    return response;
                }

                await backoff.Delay().ConfigureAwait(false);
            }
        }
    }
}