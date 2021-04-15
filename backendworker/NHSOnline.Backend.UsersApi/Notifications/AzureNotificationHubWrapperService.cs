using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    internal class AzureNotificationHubWrapperService : IAzureNotificationHubWrapperService
    {
        private readonly IEnumerable<IAzureNotificationHubWrapper> _wrappers;

        public AzureNotificationHubWrapperService(IEnumerable<IAzureNotificationHubWrapper> wrappers)
        {
            _wrappers = wrappers;
        }

        public IEnumerable<IAzureNotificationHubWrapper> All() => _wrappers;

        public IEnumerable<IAzureNotificationHubWrapper> AllFor(string nhsLoginId)
        {
            var output = _wrappers
                .Where(x => x.CanReadFor(nhsLoginId))
                .OrderByDescending(x => x.Generation)
                .ToList();

            if (!output.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(nhsLoginId), nhsLoginId);
            }

            return output;
        }

        public IAzureNotificationHubWrapper CurrentFor(string nhsLoginId)
        {
            var output = _wrappers
                .Where(x => x.CanWriteFor(nhsLoginId))
                .OrderByDescending(x => x.Generation)
                .ToList();

            if (output.Count != 1)
            {
                throw new ArgumentOutOfRangeException(nameof(nhsLoginId), nhsLoginId);
            }

            return output.First();
        }
    }
}
