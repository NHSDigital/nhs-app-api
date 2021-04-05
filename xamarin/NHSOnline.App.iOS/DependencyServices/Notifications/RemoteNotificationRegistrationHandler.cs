using System.Threading.Tasks;
using Foundation;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices.Notifications;
using NHSOnline.App.iOS.Extensions;
using NHSOnline.App.Logging;

namespace NHSOnline.App.iOS.DependencyServices.Notifications
{
    public class RemoteNotificationRegistrationHandler
    {
        private readonly TaskCompletionSource<GetPnsTokenResult> _completionSource;

        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(RemoteNotificationRegistrationHandler));

        internal RemoteNotificationRegistrationHandler(TaskCompletionSource<GetPnsTokenResult> completionSource)
        {
            _completionSource = completionSource;
        }

        internal void RegisteredForRemoteNotifications(NSData deviceToken)
        {
            var pnsToken = deviceToken.ToHexString();

            _completionSource.SetResult(new GetPnsTokenResult.Authorised(new IosGetPnsTokenResponse(pnsToken)));
        }

        internal void FailedToRegisterForRemoteNotifications(NSError error)
        {
            Logger.LogError($"Failed to register for remote notifications. | Reason: {error.LocalizedFailureReason} | Description: {error.LocalizedDescription}");

            _completionSource.SetResult(new GetPnsTokenResult.Unauthorised());
        }
    }
}