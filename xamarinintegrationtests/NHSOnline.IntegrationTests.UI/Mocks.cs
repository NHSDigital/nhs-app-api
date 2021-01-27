using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.HttpMocks;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.IntegrationTests.UI
{
    public static class Mocks
    {
        private static readonly MocksLoggerProvider _mocksLoggerProvider = new MocksLoggerProvider();

        public static PatientsCollection Patients { get; } = new PatientsCollection();

        internal static MockWebServer WebServer { get; private set; } = null!;

        internal static event EventHandler<string> Logged
        {
            add => _mocksLoggerProvider.Logged += value;
            remove => _mocksLoggerProvider.Logged -= value;
        }

        internal static void Initialize()
        {
            WebServer = MockWebServer.Start(Patients, SetupLogDelivery);
        }

        internal static async Task CleanUp()
        {
            await WebServer.DisposeAsync();
        }

        private static void SetupLogDelivery(ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddProvider(_mocksLoggerProvider);
        }
    }
}