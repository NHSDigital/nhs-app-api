using System.Collections.Generic;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Models;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Shared;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis
{
    public static class DemographicsConfigurator
    {
        private const string PathDemographics = "/emis/demographics";

        public static Mapping CreateDemographicsMapping(
            int statusCode,
            string userLinkToken,
            string sessionId,
            string endUserSessionId,
            IEnumerable<string> nhsNumbers,
            string applicationId = null,
            string version = null
        )
        {
            applicationId = applicationId ?? Configuration.EmisApplicationId;
            version = version ?? Configuration.EmisVersion;

            return new Mapping(
                new Request().ConfigureDemographicsRequest(userLinkToken, sessionId, endUserSessionId, applicationId, version),
                new Response().ConfigureDemographicsResponse(statusCode, nhsNumbers)
            );
        }

        public static Request ConfigureDemographicsRequest(
            this Request request,
            string userLinkToken,
            string sessionId,
            string endUserSessionId,
            string applicationId = null,
            string version = null
        )
        {
            applicationId = applicationId ?? Configuration.EmisApplicationId;
            version = version ?? Configuration.EmisVersion;

            return request
                .ConfigurePath(PathDemographics)
                .ConfigureMethod(Methods.Get)
                .ConfigureUserLinkToken(userLinkToken)
                .ConfigureSessionId(sessionId)
                .ConfigureEndUserSessionId(endUserSessionId)
                .ConfigureApplicationHeader(applicationId)
                .ConfigureVersionHeader(version);
        }

        public static Response ConfigureDemographicsResponse(this Response response, int statusCode, IEnumerable<string> nhsNumbers)
        {
            nhsNumbers = nhsNumbers ?? new string[0];

            var demographicsResponse = new DemographicsResponse
            {
                PatientIdentifiers = new List<PatientIdentifier>()
            };

            foreach (var nhsNumber in nhsNumbers)
            {
                demographicsResponse.PatientIdentifiers.Add(new PatientIdentifier
                {
                    IdentifierType = IdentifierType.NhsNumber,
                    IdentifierValue = nhsNumber
                });
            }

            return response.ConfigureBody(JsonConvert.SerializeObject(demographicsResponse));
        }
    }
}
