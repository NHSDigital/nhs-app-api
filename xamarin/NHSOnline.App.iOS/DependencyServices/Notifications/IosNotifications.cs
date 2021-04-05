using System;
using System.Threading.Tasks;
using CoreFoundation;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices.Notifications;
using NHSOnline.App.iOS.DependencyServices.Notifications;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using UIKit;
using UserNotifications;

[assembly: Xamarin.Forms.Dependency(typeof(IosNotifications))]
namespace NHSOnline.App.iOS.DependencyServices.Notifications
{
    public class IosNotifications: INotifications
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(IosNotifications));

        public async Task<NotificationStatus> GetDeviceNotificationsStatus()
        {
            var completionSource = new TaskCompletionSource<NotificationStatus>();

            UNUserNotificationCenter.Current.GetNotificationSettings(settings =>
                completionSource.SetResult(MapNotificationAuthorizationStatus(settings.AuthorizationStatus)));

            return await completionSource.Task.ResumeOnThreadPool();
        }

        public async Task<GetPnsTokenResult> GetPnsToken()
        {
            if (AppDelegate.Instance is null)
            {
                throw new InvalidOperationException($"{nameof(AppDelegate.Instance)} is null");
            }

            var completionSource = new TaskCompletionSource<GetPnsTokenResult>();

            AppDelegate.Instance.SetHandler(new RemoteNotificationRegistrationHandler(completionSource));

            RequestPnsToken(completionSource);

            return await completionSource.Task.ResumeOnThreadPool();
        }

        private static NotificationStatus MapNotificationAuthorizationStatus(UNAuthorizationStatus status)
        {
            var notificationStatus = NotificationStatus.denied;

            switch (status)
            {
                case UNAuthorizationStatus.Authorized:
                case UNAuthorizationStatus.Provisional:
                    notificationStatus = NotificationStatus.authorised;
                    break;
                case UNAuthorizationStatus.NotDetermined:
                    notificationStatus = NotificationStatus.notDetermined;
                    break;
                case UNAuthorizationStatus.Denied:
                case UNAuthorizationStatus.Ephemeral:
                    break;
                default:
                    Logger.LogError("Unknown UNAuthorizationStatus: {Status}", status);
                    break;
            }

            return notificationStatus;
        }

        private static void RequestPnsToken(TaskCompletionSource<GetPnsTokenResult> completionSource)
        {
            UNUserNotificationCenter.Current.RequestAuthorization(
                UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                (granted, error) =>
                {
                    if (granted)
                    {
                        DispatchQueue.MainQueue.DispatchAsync(() =>
                        {
                            UIApplication.SharedApplication.RegisterForRemoteNotifications();
                        });
                    }
                    else
                    {
                        completionSource.SetResult(new GetPnsTokenResult.Unauthorised());
                    }
                });
        }
    }
}