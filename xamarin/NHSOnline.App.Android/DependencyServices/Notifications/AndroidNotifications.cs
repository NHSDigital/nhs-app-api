using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Android.App;
using Android.Gms.Common;
using AndroidX.Core.App;
using Firebase.Messaging;
using Java.IO;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices.Notifications;
using NHSOnline.App.Droid.DependencyServices.Notifications;
using NHSOnline.App.Droid.Extensions;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using Polly;
using Polly.Retry;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidNotifications))]
namespace NHSOnline.App.Droid.DependencyServices.Notifications
{
    public class AndroidNotifications: INotifications
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AndroidNotifications));

        public bool NotificationServiceAvailable()
        {
            var googlePlayServices = GoogleApiAvailabilityLight.Instance;

            var googlePlayServicesAvailability = RetryPolicyGooglePlayServiceAvailability().Execute(() => googlePlayServices.IsGooglePlayServicesAvailable(Application.Context));

            if (googlePlayServicesAvailability != ConnectionResult.Success)
            {
                LogErrorGoogleServices(googlePlayServices, googlePlayServicesAvailability);
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

                var googlePlayServicesAvailability = RetryPolicyGooglePlayServiceAvailability().Execute(() => googlePlayServices.IsGooglePlayServicesAvailable(Application.Context));

                using var token = await RetryPolicyFirebaseMessaging()
                    .ExecuteAsync(async () => await FirebaseMessaging.Instance
                    .GetToken()
                    .ToAwaitableTask(Logger)
                    .PreserveThreadContext()).PreserveThreadContext();

                if (token == null)
                {
                    LogErrorGoogleServices(googlePlayServices, googlePlayServicesAvailability);
                    return new GetPnsTokenResult.Unauthorised();
                }

                Logger.LogInformation("Token retrieved, returning as authorised");
                return new GetPnsTokenResult.Authorised(new AndroidGetPnsTokenResponse(token.ToString()));
            }
            catch(IOException ex) when (ex.Message != null && ex.Message.Contains("TOO_MANY_REGISTRATIONS", StringComparison.InvariantCultureIgnoreCase))
            {
                Logger.LogError(ex, "Too many registrations exception thrown while trying to retrieve token");
                return new GetPnsTokenResult.Unauthorised();
            }
            catch(Exception e)
            {
                Logger.LogError(e, "Exception thrown while trying to retrieve token");
                return new GetPnsTokenResult.Unauthorised();
            }
        }

        private static void LogErrorGoogleServices(GoogleApiAvailabilityLight googlePlayServices, int googlePlayServicesAvailability, [CallerMemberName] string callerMemberName = "")
        {
            Logger.LogError("{isResolvable} issue occured from {CallerMemberName}: {PlayServicesError}",
                googlePlayServices.IsUserResolvableError(googlePlayServicesAvailability)
                    ?  "Resolvable" : "Unresolvable",
                callerMemberName,
                googlePlayServices.GetErrorString(googlePlayServicesAvailability));
        }

        private static RetryPolicy<int> RetryPolicyGooglePlayServiceAvailability()
        {
            return Policy
                .HandleResult<int>(r => r == ConnectionResult.ServiceInvalid)
                .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, retryCount) => {
                    Logger.LogError("SERVICE_INVALID -> RETRY AFTER {RetrySeconds} SECONDS", retryCount.Seconds);
                });
        }

        private static AsyncRetryPolicy RetryPolicyFirebaseMessaging()
        {
            return Policy
                .Handle<IOException>(e => e.Message != null && e.Message.Contains("SERVICE_NOT_AVAILABLE", StringComparison.Ordinal))
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, retryCount) => {
                    Logger.LogError("SERVICE_NOT_AVAILABLE -> RETRY AFTER {RetrySeconds} SECONDS", retryCount.Seconds);
                });
        }
    }
}