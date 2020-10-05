using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NHSOnline.Backend.Metrics.UnitTests
{
    [TestClass]
    public sealed class AnonymousMetricLoggerTests
    {
        [TestMethod]
        [DynamicData(nameof(AnonymousMetricLogMethods), typeof(AnonymousMetricLoggerTests), DynamicDataDisplayName = nameof(MetricLogMethodsDisplayName))]
        public async Task MetricLog_LogsTimestampFirst(Func<IAnonymousMetricLogger, Task> logMethod, string _)
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            var metricLogger = CreateMetricLogger(mockMetricContext);
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await logMethod(metricLogger);

            // Assert
            MetricLoggerAssert.AssertTimeStamp(consoleOut.ToString());
        }

        [TestMethod]
        [DynamicData(nameof(AnonymousMetricLogMethods), typeof(AnonymousMetricLoggerTests), DynamicDataDisplayName = nameof(MetricLogMethodsDisplayName))]
        public async Task MetricLog_LogsOdsCodeFromContext(Func<IAnonymousMetricLogger, Task> logMethod, string _)
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            mockMetricContext.Setup(x => x.OdsCode).Returns("A1234B");
            var metricLogger = CreateMetricLogger(mockMetricContext);
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await logMethod(metricLogger);

            // Assert
            MetricLoggerAssert.AssertContains(consoleOut.ToString(),"OdsCode=A1234B");
        }

        [TestMethod]
        [DynamicData(nameof(AnonymousMetricLogMethods), typeof(AnonymousMetricLoggerTests), DynamicDataDisplayName = nameof(MetricLogMethodsDisplayName))]
        public async Task MetricLog_LogsAction(Func<IAnonymousMetricLogger, Task> logMethod, string action)
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            var metricLogger = CreateMetricLogger(mockMetricContext);
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await logMethod(metricLogger);

            // Assert
            MetricLoggerAssert.AssertContains(consoleOut.ToString(),$"Action={action}");
        }

        [TestMethod]
        public async Task AppointmentBooked_LogsAppointmentData()
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            var metricLogger = CreateMetricLogger(mockMetricContext);
            var appointmentData = new AppointmentMetricData("SessionName", "SlotType", 200);
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await metricLogger.AppointmentBookResult(appointmentData);

            // Assert
            var splitConsoleMessage = MetricLoggerAssert.AssertSingleLine(consoleOut.ToString()).Split(" ");
            splitConsoleMessage.Should().Contain("SessionName=SessionName");
            splitConsoleMessage.Should().Contain("SlotType=SlotType");
            splitConsoleMessage.Should().Contain("StatusCode=200");
        }

        [TestMethod]
        public async Task AppointmentCancelled_LogsAppointmentData()
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            var metricLogger = CreateMetricLogger(mockMetricContext);
            var appointmentData = new AppointmentMetricData("SessionName", "SlotType", 200);
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await metricLogger.AppointmentCancelResult(appointmentData);

            // Assert
            var splitConsoleMessage = MetricLoggerAssert.AssertSingleLine(consoleOut.ToString()).Split(" ");
            splitConsoleMessage.Should().Contain("SessionName=SessionName");
            splitConsoleMessage.Should().Contain("SlotType=SlotType");
            splitConsoleMessage.Should().Contain("StatusCode=200");
        }

        private static IAnonymousMetricLogger CreateMetricLogger(Mock<IMetricContext> mockMetricContext)
        {
            var services = new ServiceCollection();
            new ServiceConfigurationModule().ConfigureServices(services, new Mock<IConfiguration>().Object);
            services.AddSingleton(mockMetricContext.Object);

            var metricLogger = services.BuildServiceProvider().GetRequiredService<IAnonymousMetricLogger>();
            return metricLogger;
        }

        public static IEnumerable<object[]> AnonymousMetricLogMethods
        {
            get
            {
                var appointmentData = new AppointmentMetricData("SessionName", "SlotType", 200);
                yield return new object[] { Method(metricLogger => metricLogger.AppointmentBookResult(appointmentData)), "AppointmentBookResult" };
                yield return new object[] { Method(metricLogger => metricLogger.AppointmentCancelResult(appointmentData)), "AppointmentCancelResult" };

                static Func<IAnonymousMetricLogger, Task> Method(Func<IAnonymousMetricLogger, Task> method) => method;
            }
        }

        public static string MetricLogMethodsDisplayName(MethodInfo methodInfo, object[] data) => $"{methodInfo.Name}({data[1]})";
    }
}
