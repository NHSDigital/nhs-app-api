using System.Threading.Tasks;
using NHSOnline.HttpMocks;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.IntegrationTests
{
    internal static class Mocks
    {
        public static PatientsCollection Patients { get; } = new PatientsCollection();

        public static MockWebServer WebServer { get; private set; } = null!;

        internal static void Initialize()
        {
            WebServer = MockWebServer.Start(Patients);
        }

        internal static async Task CleanUp()
        {
            await WebServer.DisposeAsync();
        }
    }
}