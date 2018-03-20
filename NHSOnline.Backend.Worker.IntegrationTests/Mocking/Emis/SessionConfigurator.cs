using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Models;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Shared;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis
{
    public static class SessionConfigurator
    {
        private const string PathEndUserSession = "/emis/sessions/endusersession";
        private const string PathSessions = "/emis/sessions";
        public const string BodyAccessIdentityGuid = "AccessIdentityGuid";
        public const string BodyNationalPracticeCode = "NationalPracticeCode";

        public static Mapping CreateEndUserSessionMapping(
            int statusCode,
            string endUserSessionId,
            string applicationId = null,
            string version = null
        )
        {
            applicationId = applicationId ?? Configuration.EmisApplicationId;
            version = version ?? Configuration.EmisVersion;

            return new Mapping(
                new Request().ConfigureEndUserSessionRequest(applicationId, version),
                new Response().ConfigureEndUserSessionResponse(statusCode, endUserSessionId)
            );
        }

        public static Mapping CreateSessionsMapping(
            int statusCode,
            string connectionToken,
            string odsCode,
            string sessionId,
            string linkToken,
            AssociationType associationType,
            string endUserSessionId,
            string applicationId = null,
            string version = null
        )
        {
            applicationId = applicationId ?? Configuration.EmisApplicationId;
            version = version ?? Configuration.EmisVersion;

            return new Mapping(
                new Request().ConfigureSessionsRequest(connectionToken, odsCode, endUserSessionId, applicationId, version),
                new Response().ConfigureSessionsResponse(statusCode, sessionId, linkToken, associationType)
            );
        }

        public static Request ConfigureSessionsRequest(
            this Request request,
            string connectionToken,
            string odsCode,
            string endUserSessionId = null,
            string applicationId = null,
            string version = null
        )
        {
            applicationId = applicationId ?? Configuration.EmisApplicationId;
            version = version ?? Configuration.EmisVersion;

            var bodyProperties = new Dictionary<string, string>
            {
                { BodyAccessIdentityGuid, connectionToken },
                { BodyNationalPracticeCode, odsCode }
            };

            return request
                .ConfigurePath(PathSessions)
                .ConfigureMethod(Methods.Post)
                .ConfigureEndUserSessionId(endUserSessionId)
                .ConfigureApplicationHeader(applicationId)
                .ConfigureVersionHeader(version)
                .ConfigureBody(bodyProperties);
        }

        public static Response ConfigureSessionsResponse(this Response response, int statusCode, string sessionId, string linkToken, AssociationType associationType)
        {
            var sessionsResponse = new SessionsResponse
            {
                SessionId = sessionId,
                UserPatientLinks = new List<UserPatientLink>
                {
                    new UserPatientLink
                    {
                        UserPatientLinkToken = linkToken,
                        AssociationType = associationType
                    }
                }
            };

            return response
                .ConfigureStatusCode(statusCode)
                .ConfigureBody(JsonConvert.SerializeObject(sessionsResponse));
        }

        public static Request ConfigureEndUserSessionRequest(this Request request, string applicationId = null, string version = null)
        {
            applicationId = applicationId ?? Configuration.EmisApplicationId;
            version = version ?? Configuration.EmisVersion;

            return request
                .ConfigurePath(PathEndUserSession)
                .ConfigureMethod(Methods.Post)
                .ConfigureApplicationHeader(applicationId)
                .ConfigureVersionHeader(version);
        }

        public static Response ConfigureEndUserSessionResponse(this Response response, int statusCode, string endUserSessionId)
        {
            return response
                .ConfigureStatusCode(statusCode)
                .ConfigureBody($"{{\"EndUserSessionId\":\"{ endUserSessionId }\"}}");
        }

        public static Response ConfigureEndUserSessionErrorResponse(this Response response, int statusCode, string message)
        {
            return response
                .ConfigureStatusCode(statusCode)
                .ConfigureBody(message);
        }

        public static Mapping CreateEndUserSessionMappingWithTimout(TimeSpan timeout)
        {
            return new Mapping(
                new Request().ConfigureEndUserSessionRequest(),
                new Response().ConfigureTimeoutResponse(timeout)
            );
        }

        public static Mapping CreateEndUserSessionMappingWithError(int statusCode, string message)
        {
            return new Mapping(
                new Request().ConfigureEndUserSessionRequest(),
                new Response().ConfigureEndUserSessionErrorResponse(statusCode, message)
            );
        }
    }
}
