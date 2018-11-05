// ------------------------------------------------------------------------------------------------
//  Copyright (c) Andrew Horth.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Net.Http;

namespace Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Http
{
    internal interface ISplunkHttpMessageHandlerFactory
    {
        HttpMessageHandler Create();
    }
}