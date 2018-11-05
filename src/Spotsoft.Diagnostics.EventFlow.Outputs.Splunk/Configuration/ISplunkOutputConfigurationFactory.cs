// ------------------------------------------------------------------------------------------------
//  Copyright (c) Andrew Horth.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Microsoft.Diagnostics.EventFlow;
using Microsoft.Extensions.Configuration;

namespace Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Configuration
{
    internal interface ISplunkOutputConfigurationFactory
    {
        SplunkOutputConfiguration Create(IConfiguration configuration, IHealthReporter healthReporter);
    }
}