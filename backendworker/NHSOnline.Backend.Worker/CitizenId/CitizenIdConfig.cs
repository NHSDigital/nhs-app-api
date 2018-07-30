using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public interface ICitizenIdConfig
    {
        Uri CitizenIdApiBaseUrl { get; set; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
    }

    public class CitizenIdConfig : ICitizenIdConfig
    {
        public Uri CitizenIdApiBaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public CitizenIdConfig(IConfiguration configuration, ILogger<CitizenIdConfig> logger)
        {
            var citizenIdBaseUrl = configuration.GetOrWarn("CITIZEN_ID_BASE_URL", logger);
            CitizenIdApiBaseUrl = new Uri(citizenIdBaseUrl);
            ClientId = configuration.GetOrWarn("CITIZEN_ID_CLIENT_ID", logger);
            ClientSecret = configuration.GetOrWarn("CITIZEN_ID_CLIENT_SECRET", logger);
        }
    }
}
