using System;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Logging;
using UserNotifications;

namespace NHSOnline.App.iOS.Handlers
{
    public class UserNotificationCenterHandler : UNUserNotificationCenterDelegate
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(UserNotificationCenterHandler));

        public override void WillPresentNotification (
            UNUserNotificationCenter center,
            UNNotification notification,
            Action<UNNotificationPresentationOptions> completionHandler)
        {
            Logger.LogInformation("Presenting foreground notification");
            completionHandler(UNNotificationPresentationOptions.Sound | UNNotificationPresentationOptions.Banner);
        }
    }
}