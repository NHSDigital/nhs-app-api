using System;
using System.Net;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Mocking.Models;

namespace NHSOnline.Backend.Worker.Mocking
{
    public static class ResponseConfigurator
    {
        public static Response ConfigureStatusCode(this Response response, HttpStatusCode statusCode)
        {
            response.Status = (int)statusCode;
            return response;
        }

        public static Response ConfigureBodyObject(this Response response, object body)
        {
            return response.ConfigureBody(JsonConvert.SerializeObject(body));
        }

        public static Response ConfigureBody(this Response response, string body)
        {
            response.Body = body;
            return response;
        }

        public static Response ConfigureDelay(this Response response, int secondsDelay)
        {
            response.FixedDelayMilliseconds = SecondsToMilliseconds(secondsDelay);
            return response;
        }

        private static int SecondsToMilliseconds(int seconds)
        {
            return seconds * 1000;
        }

    }
}
