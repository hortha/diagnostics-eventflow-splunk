// ------------------------------------------------------------------------------------------------
//  Copyright (c) Andrew Horth.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.EventFlow;
using Moq;
using Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Http;
using Xunit;

namespace Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Tests
{
    public class SplunkOutputTests
    {
        [Fact]
        public async Task SendEventsAsync_WhenSplunkHttpEventCollectorClientIsSuccessful_ReportsHealthy()
        {
            // Arrange
            var splunkHttpEventCollectorClient = new Mock<ISplunkHttpEventCollectorClient>();
            var healthReporter = new Mock<IHealthReporter>();
            var events = new ReadOnlyCollection<EventData>(new List<EventData>());
            var cancellationToken = new CancellationToken();

            splunkHttpEventCollectorClient
                .Setup(c => c.SendEventsAsync(events, cancellationToken))
                .Returns(Task.CompletedTask)
                .Verifiable();
            healthReporter
                .Setup(r => r.ReportHealthy(It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable();

            // Act
            var splunkOutput = new SplunkOutput(splunkHttpEventCollectorClient.Object, healthReporter.Object);
            await splunkOutput.SendEventsAsync(events, 0, cancellationToken);

            // Assert
            splunkHttpEventCollectorClient.Verify();
            healthReporter.Verify();
        }

        [Fact]
        public async Task SendEventsAsync_WhenSplunkHttpEventCollectorClientThrows_ReportsProblem()
        {
            // Arrange
            var splunkHttpEventCollectorClient = new Mock<ISplunkHttpEventCollectorClient>();
            var healthReporter = new Mock<IHealthReporter>();
            var events = new ReadOnlyCollection<EventData>(new List<EventData>());
            var cancellationToken = new CancellationToken();

            splunkHttpEventCollectorClient
                .Setup(c => c.SendEventsAsync(events, cancellationToken))
                .Throws<Exception>();
            healthReporter
                .Setup(r => r.ReportProblem(It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable();

            // Act
            var splunkOutput = new SplunkOutput(splunkHttpEventCollectorClient.Object, healthReporter.Object);
            await splunkOutput.SendEventsAsync(events, 0, cancellationToken);

            // Assert
            splunkHttpEventCollectorClient.Verify();
            healthReporter.Verify();
        }
    }
}
