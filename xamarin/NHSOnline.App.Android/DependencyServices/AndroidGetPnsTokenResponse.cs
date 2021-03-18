using NHSOnline.App.DependencyServices.Notifications;

namespace NHSOnline.App.Droid.DependencyServices
{
    public sealed class AndroidGetPnsTokenResponse : GetPnsTokenResponse
    {
        public override string DeviceType => "Android";

        public AndroidGetPnsTokenResponse(string devicePns) : base(devicePns)
        {
        }
    }
}