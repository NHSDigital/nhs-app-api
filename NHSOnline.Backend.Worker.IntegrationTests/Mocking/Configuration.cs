using System;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking
{
    public class Configuration
    {
        public const string DefaultBackendBaseUrl = "http://127.0.0.1:8080/";
        public const string DefaultEmisApplicationId = "D66BA979-60D2-49AA-BE82-AEC06356E41F";
        public const string DefaultEmisVersion = "2.1.0.0";
        public const string DefaultAdminBaseUrl = "http://127.0.0.1:8800/__admin/";
        public const string EnvironmentBackendBaseUrl = "BACKEND_BASE_URL";
        public const string EnvironmentEmisApplicationId = "EMIS_APPLICATION_ID";
        public const string EnvironmentAdminBaseUrl = "EMIS_BASE_URL";
        public const string EnvironmentEmisVersion = "EMIS_VERSION";

        static Configuration()
        {
            BackendBaseUrl = FromEnvironmentOrDefault(EnvironmentBackendBaseUrl, DefaultBackendBaseUrl);
            EmisApplicationId = FromEnvironmentOrDefault(EnvironmentEmisApplicationId, DefaultEmisApplicationId);
            EmisBaseUrl = FromEnvironmentOrDefault(EnvironmentAdminBaseUrl, DefaultAdminBaseUrl);
            EmisVersion = FromEnvironmentOrDefault(EnvironmentEmisVersion, DefaultEmisVersion);
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
