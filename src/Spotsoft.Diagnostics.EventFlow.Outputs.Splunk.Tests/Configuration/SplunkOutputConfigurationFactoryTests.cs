// ------------------------------------------------------------------------------------------------
//  Copyright (c) Andrew Horth.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Diagnostics.EventFlow;
using Microsoft.Extensions.Configuration;
using Moq;
using Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Configuration;
using Xunit;

namespace Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.Tests.Configuration
{
    public class SplunkOutputConfigurationFactoryTests
    {
        [Fact]
        public void Create_WithAllConfigItems_SetsAllProperties()
        {
            // Arrange
            var healthReporter = new Mock<IHealthReporter>();
            var builder = new ConfigurationBuilder();
            builder
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    {"type", "SplunkOutput"},
                    {"serviceBaseAddress", "https://hec.mysplunkserver.com:8088"},
                    {"authenticationToken", "B5A79AAD-D822-46CC-80D1-819F80D7BFB0"},
                    {"host", "localhost"},
                    {"index", "main"},
                    {"source", "my source"},
                    {"sourceType", "_json"},
                    {"ignoreSslCertificateErrors", "true"},
                    {"maxRetryAttempts", "5"}
                });
            var configuration = builder.Build();

            // Act
            var factory = new SplunkOutputConfigurationFactory();
            var splunkOutputConfiguration = factory.Create(configuration, healthReporter.Object);

            // Assert
            Assert.NotNull(splunkOutputConfiguration);
            Assert.Equal("SplunkOutput", splunkOutputConfiguration.Type);
            Assert.Equal("https://hec.mysplunkserver.com:8088", splunkOutputConfiguration.ServiceBaseAddress);
            Assert.Equal("B5A79AAD-D822-46CC-80D1-819F80D7BFB0", splunkOutputConfiguration.AuthenticationToken);
            Assert.Equal("localhost", splunkOutputConfiguration.Host);
            Assert.Equal("main", splunkOutputConfiguration.Index);
            Assert.Equal("my source", splunkOutputConfiguration.Source);
            Assert.Equal("_json", splunkOutputConfiguration.SourceType);
            Assert.True(splunkOutputConfiguration.IgnoreSslCertificateErrors);
            Assert.Equal(5, splunkOutputConfiguration.MaxRetryAttempts);
        }

        [Fact]
        public void Create_WithRequiredConfigItems_DoesNotThrow()
        {
            // Arrange
            var healthReporter = new Mock<IHealthReporter>();
            var builder = new ConfigurationBuilder();
            builder
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    {"type", "SplunkOutput"},
                    {"serviceBaseAddress", "https://hec.mysplunkserver.com:8088"},
                    {"authenticationToken", "B5A79AAD-D822-46CC-80D1-819F80D7BFB0"}
                });
            var configuration = builder.Build();

            // Act
            var factory = new SplunkOutputConfigurationFactory();
            var splunkOutputConfiguration = factory.Create(configuration, healthReporter.Object);

            // Assert
            Assert.NotNull(splunkOutputConfiguration);
        }

        [Fact]
        public void Create_WithEmptyOptionalConfigItems_SetsDefaultValues()
        {
            // Arrange
            var healthReporter = new Mock<IHealthReporter>();
            var builder = new ConfigurationBuilder();
            builder
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    {"type", "SplunkOutput"},
                    {"serviceBaseAddress", "https://hec.mysplunkserver.com:8088"},
                    {"authenticationToken", "B5A79AAD-D822-46CC-80D1-819F80D7BFB0"},
                    {"host", ""},
                    {"index", ""},
                    {"source", ""},
                    {"sourceType", ""}
                });
            var configuration = builder.Build();

            // Act
            var factory = new SplunkOutputConfigurationFactory();
            var splunkOutputConfiguration = factory.Create(configuration, healthReporter.Object);

            // Assert
            Assert.NotNull(splunkOutputConfiguration);
            Assert.Equal(Environment.MachineName, splunkOutputConfiguration.Host);
            Assert.Null(splunkOutputConfiguration.Index);
            Assert.Null(splunkOutputConfiguration.Source);
            Assert.Null(splunkOutputConfiguration.SourceType);
        }

        [Fact]
        public void Create_WithInvalidConfigItems_ReportsProblemAndThrows()
        {
            // Arrange
            var healthReporter = new Mock<IHealthReporter>();
            var builder = new ConfigurationBuilder();
            builder
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    {"type", "SplunkOutput"},
                    {"serviceBaseAddress", "https://hec.mysplunkserver.com:8088"},
                    {"authenticationToken", "B5A79AAD-D822-46CC-80D1-819F80D7BFB0"},
                    {"maxRetryAttempts", "ABC" }
                });
            var configuration = builder.Build();

            healthReporter
                .Setup(r => r.ReportProblem(It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable();

            // Act
            var factory = new SplunkOutputConfigurationFactory();
            Assert.ThrowsAny<Exception>(() => factory.Create(configuration, healthReporter.Object));            

            // Assert            
            healthReporter.Verify();
        }

        [Fact]
        public void Create_WithMissingRequiredServiceBaseAddress_ReportsProblemAndThrows()
        {
            // Arrange
            var healthReporter = new Mock<IHealthReporter>();
            var builder = new ConfigurationBuilder();
            builder
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    {"type", "SplunkOutput"},
                    {"authenticationToken", "B5A79AAD-D822-46CC-80D1-819F80D7BFB0"}
                });
            var configuration = builder.Build();

            healthReporter
                .Setup(r => r.ReportProblem(It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable();

            // Act
            var factory = new SplunkOutputConfigurationFactory();
            Assert.ThrowsAny<Exception>(() => factory.Create(configuration, healthReporter.Object));

            // Assert            
            healthReporter.Verify();
        }

        [Fact]
        public void Create_WithMissingRequiredAuthenticationToken_ReportsProblemAndThrows()
        {
            // Arrange
            var healthReporter = new Mock<IHealthReporter>();
            var builder = new ConfigurationBuilder();
            builder
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    {"type", "SplunkOutput"},
                    {"serviceBaseAddress", "https://hec.mysplunkserver.com:8088"}
                });
            var configuration = builder.Build();

            healthReporter
                .Setup(r => r.ReportProblem(It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable();

            // Act
            var factory = new SplunkOutputConfigurationFactory();
            Assert.ThrowsAny<Exception>(() => factory.Create(configuration, healthReporter.Object));

            // Assert            
            healthReporter.Verify();
        }
    }
}