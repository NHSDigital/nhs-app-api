using System.Collections.Generic;

namespace NHSOnline.Backend.Users.Notifications
{
    public interface IAzureNotificationHubWrapperService
    {
        public IEnumerable<IAzureNotificationHubWrapper> AllFor(string nhsLoginId);
        public IAzureNotificationHubWrapper CurrentFor(string nhsLoginId);
        public IAzureNotificationHubWrapper Hub(string path);
    }
}
