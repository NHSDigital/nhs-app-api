using System;
using NHSOnline.Backend.Worker.IntegrationTests.Features.Emis;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking
{
    public class Configuration
    {
        public const string DefaultBackendBaseUrl = "http://127.0.0.1:8080/";
        public const string DefaultAdminBaseUrl = "http://127.0.0.1:8800/__admin/";
        public const string EnvironmentBackendBaseUrl = "BACKEND_BASE_URL";
        public const string EnvironmentEmisApplicationId = "EMIS_APPLICATION_ID";
        public const string EnvironmentAdminBaseUrl = "EMIS_BASE_URL";
        public const string EnvironmentEmisVersion = "EMIS_VERSION";

        static Configuration()
        {
            BackendBaseUrl = FromEnvironmentOrDefault(EnvironmentBackendBaseUrl, DefaultBackendBaseUrl);
            EmisApplicationId = FromEnvironmentOrDefault(EnvironmentEmisApplicationId, EmisDefaults.ApplicationId);
            EmisBaseUrl = FromEnvironmentOrDefault(EnvironmentAdminBaseUrl, DefaultAdminBaseUrl);
            EmisVersion = FromEnvironmentOrDefault(EnvironmentEmisVersion, EmisDefaults.Version);
        }

        public static string EmisBaseUrl { get; }
        public static string EmisVersion { get; }
        public static string EmisApplicationId { get; }
        public static string BackendBaseUrl { get; }

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
    }
}
