using System.Threading.Tasks;
using NHSOnline.App.DependencyServices.Notifications;
using NHSOnline.App.iOS.DependencyServices;

[assembly: Xamarin.Forms.Dependency(typeof(IosNotifications))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public class IosNotifications: INotifications
    {
        public NotificationStatus GetDeviceNotificationsStatus()
        {
            return NotificationStatus.notDetermined;
        }

        public Task<GetPnsTokenResult> GetPnsToken()
        {
            //TODO: Implement getting actual PNS token from iOS
            return Task.FromResult<GetPnsTokenResult>(new GetPnsTokenResult.Authorised(new IosGetPnsTokenResponse("THISISASTUBVALUE")));
        }
    }
}