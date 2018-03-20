using System;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Models;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking
{
    public static class ResponseConfigurator
    {
        public static Response ConfigureStatusCode(this Response response, int statusCode)
        {
            response.StatusCode = statusCode;
            return response;
        }

        public static Response ConfigureBody(this Response response, string body)
        {
            response.Body = body;
            return response;
        }

        public static Response ConfigureTimeoutResponse(this Response response, TimeSpan timeout)
        {
            response.Body = $"Returned after timeout of { timeout.TotalMilliseconds } milliseconds";
            response.Delay = (int)timeout.TotalMilliseconds;
            return response;
        }
    }
}
