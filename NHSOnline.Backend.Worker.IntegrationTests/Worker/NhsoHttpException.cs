using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace NHSOnline.Backend.Worker.IntegrationTests.Worker
{
    public class NhsoHttpException : Exception
    {
        public NhsoHttpException(HttpRequestMessage request, HttpResponseMessage response)
        {
            StatusCode = response.StatusCode;
            Body = response.ToString();
            RequestUri = request.RequestUri;
            Method = request.Method;
        }

        public HttpMethod Method { get; set; }
        public string Body { get; }
        public Uri RequestUri { get; }
        public HttpStatusCode StatusCode { get; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Stubs threw an HTTP exception:");
            builder.AppendLine($"  Status code: {StatusCode}");
            builder.AppendLine($"  Request URI: {RequestUri}");
            builder.AppendLine($"  Method:      {Method}");
            builder.AppendLine("  Body:");
            builder.AppendLine(Body);
            return builder.ToString();
        }
    }
}
