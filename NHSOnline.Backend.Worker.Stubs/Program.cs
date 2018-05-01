using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Mocking;
using NHSOnline.Backend.Worker.Mocking.Emis;
using NHSOnline.Backend.Worker.Stubs.Journeys;

namespace NHSOnline.Backend.Worker.Stubs
{
    internal static class Program
    {
        private const string WiremockAuthorityEnvironmentVariable = "WIREMOCK_AUTHORITY";

        private static string WiremockAuthority
        {
            get
            {
                var wiremockAuthority = Environment.GetEnvironmentVariable(WiremockAuthorityEnvironmentVariable);
                if (string.IsNullOrWhiteSpace(wiremockAuthority))
                {
                    throw new ArgumentNullException(
                        WiremockAuthorityEnvironmentVariable,
                        "The WIREMOCK_AUTHORITY environment variable must be set to the host and port where wiremock is running (eg. localhost:8080)"
                    );
                }

                return wiremockAuthority;
            }
        }

        private static string WiremockUrl => $"http://{WiremockAuthority}/__admin/";

        public static void Main()
        {
            SetupStubs(GetConfiguration()).Wait();
        }

        private static MockingConfiguration GetConfiguration()
        {
            var emisConfiguration = new EmisConfiguration("D66BA979-60D2-49AA-BE82-AEC06356E41F", "2.1.0.0");
            return new MockingConfiguration(WiremockUrl, emisConfiguration);
        }

        private static async Task SetupStubs(MockingConfiguration configuration)
        {
            var mockingClient = new MockingClient(configuration);
            await mockingClient.ResetMappings();

            foreach (var mapping in GetAllJourneys().SelectMany(journey => journey.GetMappings()))
            {
                await mockingClient.PostMappingAsync(mapping);
            }
        }

        private static IEnumerable<Journey> GetAllJourneys()
        {
            return typeof(Journey).Assembly
                .DefinedTypes
                .Where(type => type.IsClass && !type.IsAbstract)
                .Where(type => typeof(Journey).IsAssignableFrom(type))
                .Select(Activator.CreateInstance)
                .Cast<Journey>();
        }
    }
}
