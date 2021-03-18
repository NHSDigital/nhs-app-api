namespace NHSOnline.App.DependencyServices.Notifications
{
    public abstract class GetPnsTokenResponse
    {
        public string DevicePns { get; }
        public abstract string DeviceType { get; }

        protected GetPnsTokenResponse(string devicePns)
        {
            DevicePns = devicePns;
        }
    }
}