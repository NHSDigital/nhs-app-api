using System.Collections.Generic;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    internal interface IAzureNotificationHubWrapperService
    {
        public IEnumerable<IAzureNotificationHubWrapper> AllFor(string nhsLoginId);
        public IAzureNotificationHubWrapper CurrentFor(string nhsLoginId);
        public IAzureNotificationHubWrapper Hub(string path);
    }
}
