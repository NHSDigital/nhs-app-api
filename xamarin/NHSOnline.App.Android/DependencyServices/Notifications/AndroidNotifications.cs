using System;
using System.Threading.Tasks;
using Android.App;
using Android.Gms.Common;
using Android.Gms.Extensions;
using AndroidX.Core.App;
using Firebase.Messaging;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices.Notifications;
using NHSOnline.App.Droid.DependencyServices.Notifications;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidNotifications))]
namespace NHSOnline.App.Droid.DependencyServices.Notifications
{
    public class AndroidNotifications: INotifications
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AndroidNotifications));

        public Task<NotificationStatus> GetDeviceNotificationsStatus()
        {
            var notificationManager = NotificationManagerCompat.From(Application.Context);

            var notificationsStatus = notificationManager.AreNotificationsEnabled() switch
            {
                true => NotificationStatus.authorised,
                false => NotificationStatus.denied
            };

            return Task.FromResult(notificationsStatus);
        }

        public async Task<GetPnsTokenResult> GetPnsToken()
        {
            if (!NotificationsSupported)
            {
                Logger.LogError("Notifications not supported: {PlayServicesError}", GetPlayServicesError());
                return new GetPnsTokenResult.Unauthorised();
            }

            var notificationsStatus = await GetDeviceNotificationsStatus().ResumeOnThreadPool();

            if (notificationsStatus == NotificationStatus.denied)
            {
                Logger.LogError("Notification status is denied, returning unauthorised");
                return new GetPnsTokenResult.Unauthorised();
            }

            try
            {
                var token = await FirebaseMessaging.Instance.GetToken();

                if (token == null)
                {
                    Logger.LogError("Token was not retrieved: {PlayServicesError}",GetPlayServicesError());
                    return new GetPnsTokenResult.Unauthorised();
                }

                Logger.LogInformation("Token retrieved, returning as authorised");
                return new GetPnsTokenResult.Authorised(new AndroidGetPnsTokenResponse(token.ToString()));
            }
            catch(Exception e)
            {
                Logger.LogError(e, "Exception thrown while trying to retrieve token");
                return new GetPnsTokenResult.Unauthorised();
            }
        }

        private static bool NotificationsSupported
            => GoogleApiAvailabilityLight.Instance
                .IsGooglePlayServicesAvailable(Application.Context) == ConnectionResult.Success;

        private static string GetPlayServicesError()
        {
            var resultCode = GoogleApiAvailabilityLight.Instance.IsGooglePlayServicesAvailable(Application.Context);

            if (resultCode != ConnectionResult.Success)
            {
                return GoogleApiAvailabilityLight.Instance.IsUserResolvableError(resultCode) ?
                    GoogleApiAvailabilityLight.Instance.GetErrorString(resultCode) :
                    "This device is not supported";
            }

            return "An error occurred preventing the use of push notifications";
        }
    }
}