// ------------------------------------------------------------------------------------------------
//  Copyright (c) Andrew Horth.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Diagnostics.EventFlow;
using Microsoft.Diagnostics.EventFlow.Inputs;
using Microsoft.Extensions.Logging;

namespace SplunkOutputNetCoreConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var pipeline = DiagnosticPipelineFactory.CreatePipeline(".\\eventFlowConfig.json"))
            {
                var factory = new LoggerFactory()
                    .AddEventFlow(pipeline);

                var logger = new Logger<Program>(factory);
                logger.LogInformation("Hello from {friend} for {family}!", "LoggerInput", "EventFlow");

                long iterations = 0;
                Guid correlationId = Guid.NewGuid();

                while (iterations++ <= 6)
                {                    
                    logger.LogInformation($"Correlation Id is {correlationId} which is good.", correlationId);
                    logger.LogDebug("Some stuff happened {context}", new MyContext(correlationId, $"My context description {iterations}", DateTime.Now));
                    logger.LogDebug("Some more stuff happened {context}", new Dictionary<string, string>() { { nameof(correlationId), $"My context description {iterations}" } });
                    logger.LogWarning("Warn about other stuff {context}", new { correlationId = $"My context description {iterations}", startTime = DateTime.Now });
                }

                try
                {
                    throw new Exception("Something really bad happened");
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Testing logger.Error(e, message, context) {context}", new { CorrelationId = correlationId });
                    logger.LogInformation(e, "Testing logger.Information(e, message, context) {context}", new { CorrelationId = correlationId });
                    logger.LogInformation("Testing logger.Information(message, context) {context}", new { CorrelationId = correlationId });
                }

                Console.ReadLine();
            }
        }
    }
    public class MyContext
    {
        public MyContext(Guid correlationId, string description, DateTime startTime)
        {
            CorrelationId = correlationId;
            Description = description;
            StartTime = startTime;
        }

        public Guid CorrelationId { get; }

        public string Description { get; }

        public DateTime StartTime { get; }
    }
}
