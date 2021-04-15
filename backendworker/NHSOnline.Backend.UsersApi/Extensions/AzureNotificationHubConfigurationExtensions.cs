using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.UsersApi.Extensions
{
    internal static class AzureNotificationHubConfigurationExtensions
    {
        public static IEnumerable<char> AllReadCharacters(this IEnumerable<AzureNotificationHubConfiguration> configs)
        {
            return configs
                .SelectMany(x => x.ReadCharacters)
                .Select(char.ToUpperInvariant);
        }

        public static IEnumerable<char> AllWriteCharacters(this IEnumerable<AzureNotificationHubConfiguration> configs)
        {
            return configs
                .SelectMany(x => x.WriteCharacters)
                .Select(char.ToUpperInvariant);
        }

        public static IEnumerable<int> Generations(this IEnumerable<AzureNotificationHubConfiguration> configs)
        {
            return configs
                .Select(x => x.Generation)
                .Distinct();
        }

        public static IEnumerable<char> ReadCharactersForGeneration(
            this IEnumerable<AzureNotificationHubConfiguration> configs,
            int generation
        )
        {
            return configs
                .Where(x => x.Generation == generation)
                .SelectMany(x => x.ReadCharacters)
                .Select(char.ToUpperInvariant);
        }
    }
}
