using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Tests.TestHelpers
{
    /// <summary>
    /// Implementation of HttpMessageHandler that effectively allows us to mock the responses to HttpClient operations.
    /// </summary>    
    internal class FakeResponseHandler : DelegatingHandler
    {
        private readonly Dictionary<Uri, HttpResponseMessage> fakeResponses = new Dictionary<Uri, HttpResponseMessage>();
        private readonly Dictionary<Uri, int> fakeResponseCallCount = new Dictionary<Uri, int>();

        /// <summary>
        /// Adds the fake response.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="responseMessage">The response message.</param>
        public void AddFakeResponse(Uri uri, HttpResponseMessage responseMessage)
        {
            fakeResponses.Add(uri, responseMessage);
            fakeResponseCallCount.Add(uri, 0);
        }

        /// <summary>
        /// Gets the response call count for the passed in URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public int GetResponseCallCount(Uri uri)
        {
            return fakeResponseCallCount[uri];
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (fakeResponses.ContainsKey(request.RequestUri))
            {
                fakeResponseCallCount[request.RequestUri] = fakeResponseCallCount[request.RequestUri] + 1;
                return fakeResponses[request.RequestUri];
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                RequestMessage = request
            };
        }
    }
}