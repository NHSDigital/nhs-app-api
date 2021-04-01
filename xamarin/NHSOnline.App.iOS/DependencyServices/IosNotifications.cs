using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices.Notifications;
using NHSOnline.App.iOS.DependencyServices;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using UserNotifications;

[assembly: Xamarin.Forms.Dependency(typeof(IosNotifications))]
namespace NHSOnline.App.iOS.DependencyServices
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

        public Task<GetPnsTokenResult> GetPnsToken()
        {
            //TODO: Implement getting actual PNS token from iOS
            return Task.FromResult<GetPnsTokenResult>(new GetPnsTokenResult.Authorised(new IosGetPnsTokenResponse("THISISASTUBVALUE")));
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
    }
}