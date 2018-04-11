
namespace NHSOnline.Backend.Worker.Mocking.Emis
{
    public class EmisConfiguration
    {
        internal string ApplicationId { get; }
        internal string Version { get; }

        public EmisConfiguration(string applicationId, string version)
        {
            ApplicationId = applicationId;
            Version = version;
        }
    }
}
