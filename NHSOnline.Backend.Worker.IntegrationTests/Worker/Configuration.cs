using System;
using NHSOnline.Backend.Worker.Mocking;
using NHSOnline.Backend.Worker.Mocking.Emis;

namespace NHSOnline.Backend.Worker.IntegrationTests.Worker
{
    internal static class Configuration
    {
        private const string DefaultBackendBaseUrl = "http://192.168.99.100:8080/";
        private const string DefaultWiremockBaseUrl = "http://192.168.99.100:8800/__admin/";
        private const string DefaultApplicationId = "D66BA979-60D2-49AA-BE82-AEC06356E41F";
        private const string DefaultVersion = "2.1.0.0";
        private const string EnvironmentBackendBaseUrl = "BACKEND_BASE_URL";
        private const string EnvironmentEmisApplicationId = "EMIS_APPLICATION_ID";
        private const string EnvironmentAdminBaseUrl = "EMIS_BASE_URL";
        private const string EnvironmentEmisVersion = "EMIS_VERSION";

        static Configuration()
        {
            BackendBaseUrl = FromEnvironmentOrDefault(EnvironmentBackendBaseUrl, DefaultBackendBaseUrl);
            EmisApplicationId = FromEnvironmentOrDefault(EnvironmentEmisApplicationId, DefaultApplicationId);
            WiremockBaseUrl = FromEnvironmentOrDefault(EnvironmentAdminBaseUrl, DefaultWiremockBaseUrl);
            EmisVersion = FromEnvironmentOrDefault(EnvironmentEmisVersion, DefaultVersion);
        }

        private static string WiremockBaseUrl { get; }
        private static string EmisVersion { get; }
        private static string EmisApplicationId { get; }
        internal static string BackendBaseUrl { get; }

        private static string FromEnvironmentOrDefault(string key, string defaultValue)
        {
            var result = Environment.GetEnvironmentVariable(EnvironmentBackendBaseUrl);

            if (result == null)
            {
                Console.WriteLine($"Could not extract {key} from environment.  Using default: {defaultValue}");
                result = defaultValue;
            }

            return result;
        }

        internal static MockingConfiguration ToMockingConfiguration()
        {
            var emisConfiguration = new EmisConfiguration(EmisApplicationId, EmisVersion);

            return new MockingConfiguration(WiremockBaseUrl, emisConfiguration);
        }
    }
}
