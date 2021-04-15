using System.Collections.Generic;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    internal interface IAzureNotificationHubWrapperService
    {
        public IEnumerable<IAzureNotificationHubWrapper> All();
        public IEnumerable<IAzureNotificationHubWrapper> AllFor(string nhsLoginId);
        public IAzureNotificationHubWrapper CurrentFor(string nhsLoginId);
    }
}
