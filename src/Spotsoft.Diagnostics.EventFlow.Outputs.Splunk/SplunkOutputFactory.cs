// ------------------------------------------------------------------------------------------------
//  Copyright (c) Andrew Horth.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Net.Http;
using Microsoft.Diagnostics.EventFlow;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Configuration;
using Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Http;
using Validation;

namespace Spotsoft.Diagnostics.EventFlow.Outputs.Splunk
{
    public class SplunkOutputFactory : IPipelineItemFactory<SplunkOutput>
    {
        public SplunkOutput CreateItem(IConfiguration configuration, IHealthReporter healthReporter)
        {
            Requires.NotNull(configuration, nameof(configuration));
            Requires.NotNull(healthReporter, nameof(healthReporter));

            //// TODO also target .NET Core 2.1 and use IHttpClientFactory and Polly for exponential backoff etc.
            var services = new ServiceCollection();
            services.AddSingleton<SplunkOutputConfiguration>(s =>
            {
                var configFactory = new SplunkOutputConfigurationFactory();
                return configFactory.Create(configuration, healthReporter);
            });
            services.AddSingleton<ISplunkHttpMessageHandlerFactory, SplunkHttpMessageHandlerFactory>();
            services.AddSingleton<ISplunkHttpClientFactory, SplunkHttpClientFactory>();
            services.AddTransient<HttpClient>(s => s.GetRequiredService<ISplunkHttpClientFactory>().Create());
            services.AddSingleton<ISplunkHttpEventCollectorClient, SplunkHttpEventCollectorClient>();

            var serviceProvider = services.BuildServiceProvider();
            var splunkHttpEventCollectorClient = serviceProvider.GetRequiredService<ISplunkHttpEventCollectorClient>();
            return new SplunkOutput(splunkHttpEventCollectorClient, healthReporter);
        }        
    }
}