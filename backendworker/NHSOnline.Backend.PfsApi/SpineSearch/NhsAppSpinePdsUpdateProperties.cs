namespace NHSOnline.Backend.PfsApi.SpineSearch
{
    public class NhsAppSpinePdsUpdateProperties
    {
        public string FromAsid { get; set; }

        public string ToAsid { get; set; }

        public string CpaId { get; set; }

        public string ToPartyId { get; set; }

        public bool Validate()
        {
            return !string.IsNullOrEmpty(FromAsid) &&
                   !string.IsNullOrEmpty(ToAsid) &&
                   !string.IsNullOrEmpty(CpaId) &&
                   !string.IsNullOrEmpty(ToPartyId);
        }
    }
}
