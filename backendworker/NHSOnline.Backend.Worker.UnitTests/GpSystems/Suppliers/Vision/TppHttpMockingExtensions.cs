using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using RichardSzalay.MockHttp;
using RichardSzalay.MockHttp.Matchers;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision
{
    public static class VisionHttpMockingExtensions
    {
        public static MockedRequest WhenVision(this MockHttpMessageHandler handler, HttpMethod method,
            string apiUrl)
        {
            return handler.When(method, apiUrl);
        }  
    }
}