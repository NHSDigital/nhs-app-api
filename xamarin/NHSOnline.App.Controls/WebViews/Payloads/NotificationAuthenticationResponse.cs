namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public class NotificationAuthorisedResponse
    {
        public NotificationAuthorisedResponse(string trigger, string devicePns, string deviceType)
        {
            Trigger = trigger;
            DevicePns = devicePns;
            DeviceType = deviceType;
        }

        public string Trigger { get; }
        public string DevicePns { get; }
        public string DeviceType { get; }
    }
}