using System;
using System.Net;

namespace NHSOnline.Backend.Worker.IntegrationTests.Worker
{
    public class NhsoHttpException : Exception
    {
        public NhsoHttpException(HttpStatusCode statusCode, string body)
        {
            StatusCode = statusCode;
            Body = body;
        }

        public string Body { get; }

        public HttpStatusCode StatusCode { get; }
    }
}
