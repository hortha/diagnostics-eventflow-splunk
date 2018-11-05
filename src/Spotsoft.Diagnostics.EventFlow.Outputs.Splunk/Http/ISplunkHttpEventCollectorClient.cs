// ------------------------------------------------------------------------------------------------
//  Copyright (c) Andrew Horth.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.EventFlow;

namespace Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Http
{
    public interface ISplunkHttpEventCollectorClient
    {
        Task SendEventsAsync(IReadOnlyCollection<EventData> events, CancellationToken cancellationToken);
    }
}