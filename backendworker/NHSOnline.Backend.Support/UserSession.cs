using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NHSOnline.Backend.Support
{
    [Serializable]
    public class UserSession
    {
        public string Key { get; set; }

        public string CsrfToken { get; set; }

        public GpUserSession GpUserSession { get; set; }

        public CitizenIdUserSession CitizenIdUserSession { get; set; }

        public Guid OrganDonationSessionId { get; set; }

        public string Im1ConnectionToken { get; set; }

        [JsonConverter(typeof(StringEnumConverter), false)]
        public ProofLevel ProofLevel { get; set; } = ProofLevel.P9;
    }
}