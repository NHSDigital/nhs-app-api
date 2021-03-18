using AndroidX.Core.App;
using NHSOnline.App.DependencyServices.Notifications;
using NHSOnline.App.Droid.DependencyServices;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidNotifications))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public class AndroidNotifications: INotifications
    {
        public NotificationStatus GetDeviceNotificationsStatus()
        {
            var notificationManager = NotificationManagerCompat.From(Android.App.Application.Context);
            return notificationManager.AreNotificationsEnabled() ? NotificationStatus.authorised : NotificationStatus.denied;
        }

        public GetPnsTokenResult GetPnsToken()
        {
            //TODO: Get actual PNS token from Firebase
            return new GetPnsTokenResult.Authorised(new AndroidGetPnsTokenResponse("THISISASTUBBEDVALUE"));
        }
    }
}