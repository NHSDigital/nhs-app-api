using System;
using System.Net.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    public class TppThrottlingHttpRequestException : HttpRequestException
    {
        public TppThrottlingHttpRequestException(string message) : base(message)
        {
        }

        public TppThrottlingHttpRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public TppThrottlingHttpRequestException()
        {
        }
    }
}
