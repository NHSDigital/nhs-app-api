using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public class CitizenIdConfig : ICitizenIdConfig
    {
        public Uri CitizenIdApiBaseUrl { get; set; }
        public string ClientId { get; set; }
        public string Issuer { get; set; }
        public string NhsLoginKeyPath { get; set; }
        public string NhsLoginKeyPassword { get; set; }
        public string TokenPath { get; set; } = "token";

        public CitizenIdConfig(IConfiguration configuration, ILogger<CitizenIdConfig> logger)
        {
            var citizenIdBaseUrl = configuration.GetOrWarn("CITIZEN_ID_BASE_URL", logger);
            CitizenIdApiBaseUrl = new Uri(citizenIdBaseUrl);

            ClientId = configuration.GetOrWarn("CITIZEN_ID_CLIENT_ID", logger);
            Issuer = configuration.GetOrWarn("CITIZEN_ID_JWT_ISSUER", logger);

            NhsLoginKeyPath = configuration.GetOrWarn("NHSLOGIN_KEY_PATH", logger);
            NhsLoginKeyPassword = configuration.GetOrWarn("NHSLOGIN_KEY_PASSWORD", logger);
        }
    }
}
