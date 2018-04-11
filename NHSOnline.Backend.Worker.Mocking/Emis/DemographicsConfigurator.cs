using System.Collections.Generic;
using System.Net;
using NHSOnline.Backend.Worker.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.Mocking.Models;
using NHSOnline.Backend.Worker.Mocking.Shared;

namespace NHSOnline.Backend.Worker.Mocking.Emis
{
    public class DemographicsConfigurator
    {
        private const string PathDemographics = "/emis/demographics";

        private readonly Request _request;
        private readonly Response _response;

        public DemographicsConfigurator(Request request)
        {
            _request = request;
            _response = new Response();
        }

        public static DemographicsConfigurator ForRequest(string endUserSessionId, string sessionId, string userPatientLinkToken)
        {
            return new DemographicsConfigurator(CreateDemographicsRequest(endUserSessionId, sessionId, userPatientLinkToken));
        }

        private static Request CreateDemographicsRequest(string endUserSessionId, string sessionId, string userLinkToken)
        {
            return new Request()
                .ConfigurePath(PathDemographics)
                .ConfigureMethod(Methods.Get)
                .ConfigureEndUserSessionId(endUserSessionId)
                .ConfigureSessionId(sessionId)
                .ConfigureUserLinkToken(userLinkToken)
                .ConfigureApplicationHeader()
                .ConfigureVersionHeader();
        }

        public Mapping RespondWithSuccess(string title, string firstName, string surname, IEnumerable<PatientIdentifier> patientIdentifiers)
        {
            var responseBody = new DemographicsResponse(title, firstName, surname, patientIdentifiers);

            _response
                .ConfigureStatusCode(HttpStatusCode.OK)
                .ConfigureBodyObject(responseBody);

            return new Mapping(_request, _response);
        }
    }
}
