using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Logs;
using NHSOnline.HttpMocks;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests
{
    [TestClass]
    public static class Init
    {
        private static readonly MocksLoggerProvider _mocksLoggerProvider = new MocksLoggerProvider();

        internal static MockWebServer WebServer { get; private set; } = null!;

        internal static event EventHandler<string> Logged
        {
            add => _mocksLoggerProvider.Logged += value;
            remove => _mocksLoggerProvider.Logged -= value;
        }

        [AssemblyInitialize]
#pragma warning disable CA1801 // Remove unused parameter
        public static void Initialize(TestContext context)
#pragma warning restore CA1801 // Remove unused parameter
        {
            WebServer = MockWebServer.Start(SetupLogDelivery);
        }

        [AssemblyCleanup]
        public static async Task CleanUp()
        {
            await WebServer.DisposeAsync();
        }

        private static void SetupLogDelivery(ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddProvider(_mocksLoggerProvider);
        }
    }
}