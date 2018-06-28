using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public class CitizenIdHttpClient
    {
        public CitizenIdHttpClient(HttpClient client, ICitizenIdConfig config)
        {
            client.BaseAddress = config.CitizenIdApiBaseUrl;
            Client = client;
        }

        public HttpClient Client { get; }
    }
}
