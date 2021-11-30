using System;
using Foundation;
using Microsoft.Extensions.Logging;
using NHSOnline.App.iOS.Extensions;
using NHSOnline.App.Logging;
using UIKit;

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
            if (UIApplication.SharedApplication.IsBrowserOverlayActive())
            {
                return;
            }

            try
            {
                var aps = (NSDictionary) userInfo["aps"];

                if (aps.Count == 0)
                {
                    Logger.LogInformation("aps dictionary is empty");
                    return;
                }

                if (aps["url"] is null)
                {
                    Logger.LogInformation("aps dictionary does not contain a url");
                    return;
                }

                var url = aps["url"].ToString();

                if (string.IsNullOrWhiteSpace(url))
                {
                    Logger.LogInformation("Url is empty");
                    return;
                }

                Logger.LogInformation("Notification contains url {url}", url);
                _nhsApp.HandleDeeplink(url);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to ReceiveRemoteNotification");
            }
        }
    }
}