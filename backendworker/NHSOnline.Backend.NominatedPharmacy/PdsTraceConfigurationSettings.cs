using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public class PdsTraceConfigurationSettings
    {
        public string FromAddress { get; set; }

        public string ToAddress { get; set; }
        
        public string FromAsid { get; set; }

        public string ToAsid { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(FromAddress))
            {
                throw new ConfigurationNotFoundException(nameof(FromAddress));
            }

            if (string.IsNullOrEmpty(ToAddress))
            {
                throw new ConfigurationNotFoundException(nameof(ToAddress));
            }

            if (string.IsNullOrEmpty(FromAsid))
            {
                throw new ConfigurationNotFoundException(nameof(FromAsid));
            }

            if (string.IsNullOrEmpty(ToAsid))
            {
                throw new ConfigurationNotFoundException(nameof(ToAsid));
            }
        }
    }
}
