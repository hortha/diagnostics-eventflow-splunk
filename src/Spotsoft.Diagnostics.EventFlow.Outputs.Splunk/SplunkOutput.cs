// ------------------------------------------------------------------------------------------------
//  Copyright (c) Andrew Horth.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.EventFlow;
using Microsoft.Diagnostics.EventFlow.Utilities;
using Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Http;
using Validation;

namespace Spotsoft.Diagnostics.EventFlow.Outputs.Splunk
{
    public class SplunkOutput : IOutput
    {        
        private readonly ISplunkHttpEventCollectorClient splunkHttpEventCollectorClient;
        private readonly IHealthReporter healthReporter;      

        public SplunkOutput(
            ISplunkHttpEventCollectorClient splunkHttpEventCollectorClient, 
            IHealthReporter healthReporter)
        {
            Requires.NotNull(splunkHttpEventCollectorClient, nameof(splunkHttpEventCollectorClient));
            Requires.NotNull(healthReporter, nameof(healthReporter));

            this.splunkHttpEventCollectorClient = splunkHttpEventCollectorClient;
            this.healthReporter = healthReporter;
        }

        public async Task SendEventsAsync(IReadOnlyCollection<EventData> events, long transmissionSequenceNumber, CancellationToken cancellationToken)
        {
            try
            {
                await splunkHttpEventCollectorClient.SendEventsAsync(events, cancellationToken);
                healthReporter.ReportHealthy();
            }
            catch (Exception e)
            {
                ErrorHandlingPolicies.HandleOutputTaskError(e, () =>
                {
                    string errorMessage = $"{nameof(SplunkOutput)}: An error occurred while sending data to Splunk. Exception: {e}";
                    healthReporter.ReportProblem(errorMessage, EventFlowContextIdentifiers.Output);
                });
            }            
        }
    }
}