// ------------------------------------------------------------------------------------------------
//  Copyright (c) Andrew Horth.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using Microsoft.Diagnostics.EventFlow;
using Microsoft.Extensions.Configuration;

namespace Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Configuration
{
    internal class SplunkOutputConfigurationFactory : ISplunkOutputConfigurationFactory
    {
        public SplunkOutputConfiguration Create(IConfiguration configuration, IHealthReporter healthReporter)
        {
            var splunkOutputConfiguration = new SplunkOutputConfiguration();
            try
            {
                configuration.Bind(splunkOutputConfiguration);
            }
            catch
            {
                healthReporter.ReportProblem($"Invalid {nameof(SplunkOutput)} configuration encountered: '{configuration}'",
                    EventFlowContextIdentifiers.Configuration);
                throw;
            }

            if (string.IsNullOrWhiteSpace(splunkOutputConfiguration.ServiceBaseAddress))
            {
                var errorMessage = $"{nameof(SplunkOutput)}: 'serviceBaseAddress' configuration parameter is not set";
                healthReporter.ReportProblem(errorMessage, EventFlowContextIdentifiers.Configuration);
                throw new Exception(errorMessage);
            }

            if (string.IsNullOrWhiteSpace(splunkOutputConfiguration.AuthenticationToken))
            {
                var errorMessage = $"{nameof(SplunkOutput)}: 'authenticationToken' configuration parameter is not set";
                healthReporter.ReportProblem(errorMessage, EventFlowContextIdentifiers.Configuration);
                throw new Exception(errorMessage);
            }

            splunkOutputConfiguration.Host = !string.IsNullOrWhiteSpace(splunkOutputConfiguration.Host) ? splunkOutputConfiguration.Host : Environment.MachineName;
            splunkOutputConfiguration.Index = !string.IsNullOrWhiteSpace(splunkOutputConfiguration.Index) ? splunkOutputConfiguration.Index : null;
            splunkOutputConfiguration.Source = !string.IsNullOrWhiteSpace(splunkOutputConfiguration.Source) ? splunkOutputConfiguration.Source : null;
            splunkOutputConfiguration.SourceType = !string.IsNullOrWhiteSpace(splunkOutputConfiguration.SourceType) ? splunkOutputConfiguration.SourceType : null;

            return splunkOutputConfiguration;
        }
    }
}