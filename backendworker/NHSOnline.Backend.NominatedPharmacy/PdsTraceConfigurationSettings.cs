namespace NHSOnline.Backend.NominatedPharmacy
{
    public class PdsTraceConfigurationSettings
    {
        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        public string FromAsid { get; set; }

        public string ToAsid { get; set; }

        public bool Validate()
        {
            return !string.IsNullOrEmpty(FromAddress) &&
                   !string.IsNullOrEmpty(ToAddress) &&
                   !string.IsNullOrEmpty(FromAsid) &&
                   !string.IsNullOrEmpty(ToAsid);
        }
    }
}
