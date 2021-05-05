using System;
using Foundation;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Logging;

namespace NHSOnline.App.iOS.DependencyServices.Notifications
{
    public class RemoteNotificationReceivedHandler
    {
        private readonly NhsApp _nhsApp;

        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(RemoteNotificationReceivedHandler));

        internal RemoteNotificationReceivedHandler(NhsApp nhsApp)
        {
            _nhsApp = nhsApp;
        }

        internal void DidReceiveRemoteNotification(NSDictionary userInfo)
        {
            try
            {
                var aps = (NSDictionary) userInfo["aps"];

                if (aps.Count == 0)
                {
                    Logger.LogInformation(nameof(RemoteNotificationReceivedHandler), $"aps dictionary is empty");
                    return;
                }

                if (aps["url"] is null)
                {
                    Logger.LogInformation(nameof(RemoteNotificationReceivedHandler), $"aps dictionary does not contain a url");
                    return;
                }

                var url = aps["url"].ToString();

                if (string.IsNullOrWhiteSpace(url))
                {
                    Logger.LogInformation(nameof(RemoteNotificationReceivedHandler), $"Url is empty");
                    return;
                }

                Logger.LogInformation(nameof(RemoteNotificationReceivedHandler), $"Notification contains url {url}");
                _nhsApp.HandleDeeplink(url);
            }
            catch (Exception e)
            {
                Logger.LogError(nameof(RemoteNotificationReceivedHandler), $"Exception occured: {e}");
            }
        }
    }
}