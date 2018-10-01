using System;
using System.Net.Http;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision
{
    public static class VisionHttpMockingExtensions
    {
        public static MockedRequest WhenVision(this MockHttpMessageHandler handler, HttpMethod method,
            Uri apiUrl)
        {
            return handler.When(method, apiUrl.ToString());
        }  
    }
}