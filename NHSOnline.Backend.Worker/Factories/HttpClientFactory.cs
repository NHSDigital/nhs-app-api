using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Interfaces;

namespace NHSOnline.Backend.Worker.Factories
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient Create()
        {
            return new HttpClient();
        }
    }
}
