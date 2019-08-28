namespace NHSOnline.Backend.Support.Settings
{
    public interface IHttpTimeoutConfigurationSettings 
    {
        int DefaultHttpTimeoutSeconds { get; set; }
    }
}