using System;
using System.Threading.Tasks;
using Android.App;
using Android.Gms.Common;
using AndroidX.Core.App;
using Firebase.Messaging;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices.Notifications;
using NHSOnline.App.Droid.DependencyServices.Notifications;
using NHSOnline.App.Droid.Extensions;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidNotifications))]
namespace NHSOnline.App.Droid.DependencyServices.Notifications
{
    public class AndroidNotifications: INotifications
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AndroidNotifications));

        public bool NotificationServiceAvailable()
        {
            var googlePlayServices = GoogleApiAvailabilityLight.Instance;
            var googlePlayServicesAvailability = googlePlayServices.IsGooglePlayServicesAvailable(Application.Context);

            if (googlePlayServicesAvailability != ConnectionResult.Success)
            {
                Logger.LogError("{isResolvable} issue occured from google services: {PlayServicesError}",
                        googlePlayServices.IsUserResolvableError(googlePlayServicesAvailability)
                            ?  "Resolvable" : "Unresolvable",
                        googlePlayServices.GetErrorString(googlePlayServicesAvailability));

                return false;
            }

            return true;
        }

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
            try
            {
                var notificationsStatus = await GetDeviceNotificationsStatus().PreserveThreadContext();

                if (notificationsStatus == NotificationStatus.denied)
                {
                    Logger.LogError("Notification status is denied, returning unauthorised");
                    return new GetPnsTokenResult.Unauthorised();
                }

                var googlePlayServices = GoogleApiAvailabilityLight.Instance;
                var googlePlayServicesAvailability = googlePlayServices.IsGooglePlayServicesAvailable(Application.Context);

                using var token = await FirebaseMessaging.Instance
                    .GetToken()
                    .ToAwaitableTask(Logger)
                    .PreserveThreadContext();

                if (token == null)
                {
                    Logger.LogError("{isResolvable} issue occured from retrieving the token: {PlayServicesError}",
                        googlePlayServices.IsUserResolvableError(googlePlayServicesAvailability)
                            ?  "Resolvable" : "Unresolvable",
                        googlePlayServices.GetErrorString(googlePlayServicesAvailability));
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
    }
}