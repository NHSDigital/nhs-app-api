using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Users.Extensions;

namespace NHSOnline.Backend.Users
{
    public class AzureNotificationHubConfigurationValidator
    {
        private const string FullRangeOfCharacters = "0123456789ABCDEF";

        private readonly ILogger _logger;

        public AzureNotificationHubConfigurationValidator(ILogger logger)
        {
            _logger = logger;
        }

        public void Validate(IReadOnlyCollection<AzureNotificationHubConfiguration> configs)
        {
            var errors = new List<string>();

            errors.AddRange(FullRangeOfCharacters
                .Where(x => !configs.AllReadCharacters().Contains(x))
                .Select(x => $"No entry for read character [{x}] found"));

            errors.AddRange(FullRangeOfCharacters
                .Where(x => !configs.AllWriteCharacters().Contains(x))
                .Select(x => $"No entry for write character [{x}] found"));

            errors.AddRange(FullRangeOfCharacters
                .Where(x => configs.AllWriteCharacters().Count(wc => wc == x) > 1)
                .Select(x => $"Multiple entries for write character [{x}] found"));

            foreach (var config in configs)
            {
                errors.AddRange(config.WriteCharacters.Intersect(FullRangeOfCharacters)
                    .Where(x => !config.ReadCharacters.Contains(x, StringComparison.InvariantCulture))
                    .Select(x => $"Entry for write character [{x}] found that does not permit reading"));
            }

            foreach (var generation in configs.Generations())
            {
                errors.AddRange(FullRangeOfCharacters
                    .Where(x => configs.ReadCharactersForGeneration(generation).Count(rc => rc == x) > 1)
                    .Select(x => $"Multiple entries for read character [{x}] found in generation [{generation}]"));
            }

            if (!errors.Any())
            {
                return;
            }

            _logger.LogCritical(string.Join(", ", errors));
            throw new ConfigurationNotValidException(errors);
        }
    }
}
