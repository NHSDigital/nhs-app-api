using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.HttpMocks;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.IntegrationTests.UI
{
    public static class Mocks
    {
        private static readonly MocksLoggerProvider MocksLoggerProvider = new();

        public static PatientsCollection Patients { get; } = new();

        internal static MockWebServer WebServer { get; private set; } = null!;

        internal static event EventHandler<string> Logged
        {
            add => MocksLoggerProvider.Logged += value;
            remove => MocksLoggerProvider.Logged -= value;
        }

        internal static async Task Initialize()
        {
            WebServer = await MockWebServer.Start(Patients, SetupLogDelivery);
        }

        internal static async Task CleanUp()
        {
            await WebServer.DisposeAsync();
        }

        private static void SetupLogDelivery(ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddProvider(MocksLoggerProvider);
        }
    }
}