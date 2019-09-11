namespace NHSOnline.Backend.GpSystems.Session
{
    public class SessionConfigurationSettings
    {
        public bool ProxyEnabled { get; set; }
        public SessionConfigurationSettings(bool proxyEnabled)
        {
            ProxyEnabled = proxyEnabled;
        }
    }
}