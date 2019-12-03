using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public class CitizenIdConfig : ICitizenIdConfig
    {
        public Uri CitizenIdApiBaseUrl { get; set; }
        public CitizenIdAuthenticationType AuthenticationType { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Issuer { get; set; }
        public string NhsLoginKeyPath { get; set; }
        public string NhsLoginKeyPassword { get; set; }
        public string TokenPath { get; set; } = "token";

        public CitizenIdConfig(IConfiguration configuration, ILogger<CitizenIdConfig> logger)
        {
            AuthenticationType = GetAuthenticationType(configuration, logger);
            
            var citizenIdBaseUrl = configuration.GetOrWarn("CITIZEN_ID_BASE_URL", logger);
            CitizenIdApiBaseUrl = new Uri(citizenIdBaseUrl);
            
            ClientId = configuration.GetOrWarn("CITIZEN_ID_CLIENT_ID", logger);
            Issuer = configuration.GetOrWarn("CITIZEN_ID_JWT_ISSUER", logger);
            
            if (AuthenticationType == CitizenIdAuthenticationType.Basic)
            {
                ClientSecret = configuration.GetOrWarn("CITIZEN_ID_CLIENT_SECRET", logger);
            }
            else if (AuthenticationType == CitizenIdAuthenticationType.Jwt)
            {
                NhsLoginKeyPath = configuration.GetOrWarn("NHSLOGIN_KEY_PATH", logger);
                NhsLoginKeyPassword = configuration.GetOrWarn("NHSLOGIN_KEY_PASSWORD", logger);
            }
        }
        
        private static CitizenIdAuthenticationType GetAuthenticationType(IConfiguration configuration, ILogger<CitizenIdConfig> logger)
        {
            var authenticationTypeString = configuration.GetOrThrow("CITIZEN_ID_AUTHENTICATION_TYPE", logger);
            CitizenIdAuthenticationType authenticationType;
            switch (authenticationTypeString.ToUpperInvariant())
            {
                case "BASIC":
                    authenticationType = CitizenIdAuthenticationType.Basic;
                    break;
                case "JWT":
                    authenticationType = CitizenIdAuthenticationType.Jwt;
                    break;
                default:
                    authenticationType = CitizenIdAuthenticationType.Basic;
                    break;
            }
            return authenticationType;
        }
    }
}
