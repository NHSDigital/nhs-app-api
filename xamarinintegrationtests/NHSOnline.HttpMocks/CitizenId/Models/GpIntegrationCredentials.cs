using System.Text.Json.Serialization;

// ReSharper disable UnusedMember.Global

namespace NHSOnline.HttpMocks.CitizenId.Models
{
    internal sealed class GpIntegrationCredentials
    {
        public GpIntegrationCredentials(string odsCode)
        {
            OdsCode = odsCode;
        }

        [JsonPropertyName("gp_ods_code")]
        public string OdsCode { get; }
    }
}