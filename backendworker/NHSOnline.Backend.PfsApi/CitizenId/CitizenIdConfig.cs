using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.CitizenId
{
    public interface ICitizenIdConfig
    {
        Uri CitizenIdApiBaseUrl { get; set; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string Issuer { get; set; }
    }

    public class CitizenIdConfig : ICitizenIdConfig
    {
        public Uri CitizenIdApiBaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Issuer { get; set; }

        public CitizenIdConfig(IConfiguration configuration, ILogger<CitizenIdConfig> logger)
        {
            var citizenIdBaseUrl = configuration.GetOrWarn("CITIZEN_ID_BASE_URL", logger);
            CitizenIdApiBaseUrl = new Uri(citizenIdBaseUrl);
            ClientId = configuration.GetOrWarn("CITIZEN_ID_CLIENT_ID", logger);
            ClientSecret = configuration.GetOrWarn("CITIZEN_ID_CLIENT_SECRET", logger);
            Issuer = configuration.GetOrWarn("CITIZEN_ID_JWT_ISSUER", logger);
        }
    }
}
