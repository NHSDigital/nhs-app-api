using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Models;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis
{
    public static class EmisRequestConfigurator
    {
        private const string HeaderApplicationId = "X-API-ApplicationId";
        private const string HeaderEndUserSessionId = "X-API-EndUserSessionId";
        private const string HeaderSessionId = "X-API-SessionId";
        private const string HeaderVersion = "X-API-Version";
        private const string QueryUserPatientLinkToken = "userPatientLinkToken";

        public static Request ConfigureApplicationHeader(this Request request, string applicationId = null)
        {
            applicationId = applicationId ?? Configuration.EmisApplicationId;
            request.ConfigureHeader(HeaderApplicationId, applicationId);
            return request;
        }

        public static Request ConfigureEndUserSessionId(this Request request, string endUserSessionId)
        {
            request.ConfigureHeader(HeaderEndUserSessionId, endUserSessionId);
            return request;
        }

        public static Request ConfigureSessionId(this Request request, string sessionId)
        {
            request.ConfigureHeader(HeaderSessionId, sessionId);
            return request;
        }

        public static Request ConfigureUserLinkToken(this Request request, string userLinkToken)
        {
            request.ConfigureQueryParameter(QueryUserPatientLinkToken, userLinkToken);
            return request;
        }

        public static Request ConfigureVersionHeader(this Request request, string version = null)
        {
            version = version ?? Configuration.EmisVersion;
            request.ConfigureHeader(HeaderVersion, version);
            return request;
        }
    }
}
