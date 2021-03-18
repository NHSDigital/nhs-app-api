namespace NHSOnline.App.DependencyServices.Notifications
{
    public class NotificationAuthorisedResponse
    {
        public NotificationAuthorisedResponse(string trigger, GetPnsTokenResult.Authorised result)
        {
            Trigger = trigger;
            DevicePns = result.Response.DevicePns;
            DeviceType = result.Response.DeviceType;
        }

        public string Trigger { get; }
        public string DevicePns { get; }
        public string DeviceType { get; }
    }
}