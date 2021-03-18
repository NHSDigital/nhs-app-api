using NHSOnline.App.DependencyServices.Notifications;

namespace NHSOnline.App.iOS.DependencyServices
{
    public class IosGetPnsTokenResponse : GetPnsTokenResponse
    {
        public override string DeviceType => "Ios";

        public IosGetPnsTokenResponse(string devicePns) : base(devicePns)
        {
        }
    }
}