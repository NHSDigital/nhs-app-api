using System;
using Microsoft.Extensions.Configuration;

namespace NHSOnline.Backend.Worker.Suppliers.Emis
{
    public interface IEmisConfig
    {
        Uri BaseUrl { get; set; }
        string ApplicationId { get; set; }
        string Version { get; set; }
    }

    public class EmisConfig : IEmisConfig
    {
        public Uri BaseUrl { get; set; }
        public string ApplicationId { get; set; }
        public string Version { get; set; }

        public EmisConfig(IConfiguration configuration)
        {
            BaseUrl = new Uri(configuration["EMIS_BASE_URL"]);
            ApplicationId = configuration["EMIS_APPLICATION_ID"];
            Version = configuration["EMIS_VERSION"];
        }
    }
}