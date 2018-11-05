// ------------------------------------------------------------------------------------------------
//  Copyright (c) Andrew Horth.  All rights reserved.
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Microsoft.Diagnostics.EventFlow.Configuration;

namespace Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Configuration
{
    public class SplunkOutputConfiguration : ItemConfiguration
    {
        public string ServiceBaseAddress { get; set; }

        public string AuthenticationToken { get; set; }        

        public string Host { get; set; }

        public string Index { get; set; }
        
        public string Source { get; set; }

        public string SourceType { get; set; }

        public bool IgnoreSslCertificateErrors { get; set; }

        public int MaxRetryAttempts { get; set; }        

        public SplunkOutputConfiguration DeepClone()
        {
            var other = new SplunkOutputConfiguration()
            {
                ServiceBaseAddress =  this.ServiceBaseAddress,
                AuthenticationToken = this.AuthenticationToken,
                Host = this.Host,
                Index =  this.Index,
                Source = this.Source,
                SourceType = this.SourceType
            };

            return other;
        }
    }
}