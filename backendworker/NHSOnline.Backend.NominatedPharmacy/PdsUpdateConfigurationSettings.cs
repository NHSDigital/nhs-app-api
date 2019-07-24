using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public class PdsUpdateConfigurationSettings
    {
        public string FromAsid { get; set; }

        public string ToAsid { get; set; }

        public string CpaId { get; set; }

        public string FromPartyId { get; set; }

        public string ToPartyId { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(FromAsid))
            {
                throw new ConfigurationNotFoundException(nameof(FromAsid));
            }

            if (string.IsNullOrEmpty(ToAsid))
            {
                throw new ConfigurationNotFoundException(nameof(ToAsid));
            }

            if (string.IsNullOrEmpty(CpaId))
            {
                throw new ConfigurationNotFoundException(nameof(CpaId));
            }

            if (string.IsNullOrEmpty(FromPartyId))
            {
                throw new ConfigurationNotFoundException(nameof(FromPartyId));
            }

            if (string.IsNullOrEmpty(ToPartyId))
            {
                throw new ConfigurationNotFoundException(nameof(ToPartyId));
            }
        }
    }
}
