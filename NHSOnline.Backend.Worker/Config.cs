using System;
using Microsoft.Extensions.Configuration;
using NHSOnline.Backend.Worker.Interfaces;

namespace NHSOnline.Backend.Worker
{
    public class Config : IConfig
    {
        public string StubsEndpointUrl { get; set; }

        public Config(IConfiguration configuration)
        {
            StubsEndpointUrl = configuration["STUBS_ENDPOINT_URL"];
        }
    }
}
