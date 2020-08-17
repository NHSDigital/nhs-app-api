namespace NHSOnline.Backend.Support.Settings
{
    public class HttpTimeoutConfigurationSettings : IHttpTimeoutConfigurationSettings
    {
        public HttpTimeoutConfigurationSettings(int defaultHttpTimeoutSeconds)
        {
            DefaultHttpTimeoutSeconds = defaultHttpTimeoutSeconds;
        }

        public int DefaultHttpTimeoutSeconds { get; set; }
    }
}
