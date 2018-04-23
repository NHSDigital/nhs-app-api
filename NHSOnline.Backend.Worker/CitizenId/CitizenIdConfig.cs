using System;
using Microsoft.Extensions.Configuration;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public interface ICitizenIdConfig
    {
        Uri CitizenIdApiBaseUrl { get; set; }
        Uri NhsWebAppBaseUrl { get; set; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
    }

    public class CitizenIdConfig : ICitizenIdConfig
    {
        public Uri CitizenIdApiBaseUrl { get; set; }
        public Uri NhsWebAppBaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public CitizenIdConfig(IConfiguration configuration)
        {
            CitizenIdApiBaseUrl = new Uri(configuration["CITIZEN_ID_BASE_URL"]);
            NhsWebAppBaseUrl = new Uri(configuration["NHS_WEB_APP_BASE_URL"]);
            ClientId = configuration["CITIZEN_ID_CLIENT_ID"];
            ClientSecret = configuration["CITIZEN_ID_CLIENT_SECRET"];
        }
    }
}
