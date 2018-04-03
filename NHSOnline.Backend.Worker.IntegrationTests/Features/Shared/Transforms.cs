using System;
using System.Collections.Generic;
using System.Net;
using TechTalk.SpecFlow;

namespace NHSOnline.Backend.Worker.IntegrationTests.Features.Shared
{
    [Binding]
    public class Transforms
    {
        private readonly Dictionary<string, HttpStatusCode> _errorMapping = new Dictionary<string, HttpStatusCode>()
        {
            { "bad gateway", HttpStatusCode.BadGateway },
            { "bad request", HttpStatusCode.BadRequest },
            { "gateway timeout", HttpStatusCode.GatewayTimeout },
            { "not found", HttpStatusCode.NotFound },
            { "internal server error", HttpStatusCode.InternalServerError },
            { "conflict", HttpStatusCode.Conflict },
            { "forbidden", HttpStatusCode.Forbidden },
            { "service unavailable", HttpStatusCode.ServiceUnavailable}
        };

        [StepArgumentTransformation(@"""(.*)"" error")]
        public HttpStatusCode HttpStatucCodeTransform(string errorName)
        {
            if (_errorMapping.TryGetValue(errorName.ToLower(), out var expectedStatusCode))
            {
                return expectedStatusCode;
            }

            throw new ArgumentException($"Could not identify an HTTP status code named: {errorName}", nameof(errorName));
        }
    }
}
