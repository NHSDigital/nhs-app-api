using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Metrics.UnitTests
{
    [TestClass]
    public sealed class MetricLoggerTests
    {
        [TestMethod]
        [DynamicData(nameof(MetricLogMethods), typeof(MetricLoggerTests), DynamicDataDisplayName = nameof(MetricLogMethodsDisplayName))]
        public async Task MetricLog_LogsTimestampFirst(Func<IMetricLogger, Task> logMethod, string _)
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            var metricLogger = CreateMetricLogger(mockMetricContext);
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await logMethod(metricLogger);

            // Assert
            AssertSingleLine(consoleOut.ToString())
                .Split(" ")[0].Should()
                .MatchRegex(@"^Timestamp=\d\d\d\d-\d\d-\d\dT\d\d:\d\d:\d\d\:\d\d\d$");
        }

        [TestMethod]
        [DynamicData(nameof(MetricLogMethods), typeof(MetricLoggerTests), DynamicDataDisplayName = nameof(MetricLogMethodsDisplayName))]
        public async Task MetricLog_LogsNhsLoginIdFromContext(Func<IMetricLogger, Task> logMethod, string _)
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            mockMetricContext.Setup(x => x.NhsLoginId).Returns("123-456-789");
            var metricLogger = CreateMetricLogger(mockMetricContext);
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await logMethod(metricLogger);

            // Assert
            AssertSingleLine(consoleOut.ToString()).Split(" ").Should().Contain("NhsLoginId=123-456-789");
        }

        [TestMethod]
        [DynamicData(nameof(MetricLogMethods), typeof(MetricLoggerTests), DynamicDataDisplayName = nameof(MetricLogMethodsDisplayName))]
        public async Task MetricLog_LogsOdsCodeFromContext(Func<IMetricLogger, Task> logMethod, string _)
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            mockMetricContext.Setup(x => x.OdsCode).Returns("A1234B");
            var metricLogger = CreateMetricLogger(mockMetricContext);
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await logMethod(metricLogger);

            // Assert
            AssertSingleLine(consoleOut.ToString()).Split(" ").Should().Contain("OdsCode=A1234B");
        }

        [TestMethod]
        [DynamicData(nameof(MetricLogMethods), typeof(MetricLoggerTests), DynamicDataDisplayName = nameof(MetricLogMethodsDisplayName))]
        public async Task MetricLog_LogsProofLevelFromContext(Func<IMetricLogger, Task> logMethod, string _)
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            mockMetricContext.Setup(x => x.ProofLevel).Returns(ProofLevel.P9);
            var metricLogger = CreateMetricLogger(mockMetricContext);
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await logMethod(metricLogger);

            // Assert
            AssertSingleLine(consoleOut.ToString()).Split(" ").Should().Contain("ProofLevel=P9");
        }

        [TestMethod]
        [DynamicData(nameof(MetricLogMethods), typeof(MetricLoggerTests), DynamicDataDisplayName = nameof(MetricLogMethodsDisplayName))]
        public async Task MetricLog_LogsAction(Func<IMetricLogger, Task> logMethod, string action)
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            var metricLogger = CreateMetricLogger(mockMetricContext);
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await logMethod(metricLogger);

            // Assert
            AssertSingleLine(consoleOut.ToString()).Split(" ").Should().Contain($"Action={action}");
        }

        private static IMetricLogger CreateMetricLogger(Mock<IMetricContext> mockMetricContext)
        {
            var services = new ServiceCollection();
            new ServiceConfigurationModule().ConfigureServices(services, new Mock<IConfiguration>().Object);
            services.AddSingleton(mockMetricContext.Object);

            var metricLogger = services.BuildServiceProvider().GetRequiredService<IMetricLogger>();
            return metricLogger;
        }

        private string AssertSingleLine(string consoleOut)
        {
            return consoleOut
                .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Should()
                .ContainSingle("a single metric log is expected")
                .Subject.Trim();
        }

        private static IEnumerable<object[]> MetricLogMethods
        {
            get
            {
                yield return new object[]{ Method(metricLogger => metricLogger.Login()), "Login" };
                yield return new object[]{ Method(metricLogger => metricLogger.UpliftStarted()), "UpliftStarted" };
                yield return new object[]{ Method(metricLogger => metricLogger.UserResearchOptIn()), "UserResearchOptIn" };
                yield return new object[]{ Method(metricLogger => metricLogger.UserResearchOptOut()), "UserResearchOptOut" };
                yield return new object[]{ Method(metricLogger => metricLogger.TermsAndConditionsInitialConsent()), "TermsAndConditionsInitialConsent" };

                static Func<IMetricLogger, Task> Method(Func<IMetricLogger, Task> method) => method;
            }
        }

        public static string MetricLogMethodsDisplayName(MethodInfo methodInfo, object[] data) => $"{methodInfo.Name}({data[1]})";

        internal class CaptureConsoleOut : IDisposable
        {
            private readonly StringWriter _consoleOutput = new StringWriter();
            private readonly TextWriter _originalConsoleOutput;

            public CaptureConsoleOut()
            {
                _originalConsoleOutput = Console.Out;
                Console.SetOut(_consoleOutput);
            }

            public void Dispose()
            {
                Console.SetOut(_originalConsoleOutput);
                Console.Write(ToString());
                _consoleOutput.Dispose();
            }

            public override string ToString() => _consoleOutput.ToString();
        }
    }
}
