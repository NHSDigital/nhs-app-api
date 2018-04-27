using System.Net;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.Mocking.Models;
using NHSOnline.Backend.Worker.Mocking.Shared;

namespace NHSOnline.Backend.Worker.Mocking.Emis
{
    public class SessionConfigurator
    {
        private const string PathSessions = "/emis/sessions";

        private readonly Request _request;

        private SessionConfigurator(Request request)
        {
            _request = request;
        }

        public static SessionConfigurator ForRequest(string connectionToken, string odsCode, string endUserSessionId)
        {
            return new SessionConfigurator(CreateSessionsRequest(connectionToken, odsCode, endUserSessionId));
        }

        private static Request CreateSessionsRequest(
            string connectionToken,
            string odsCode,
            string endUserSessionId)
        {
            var bodyProperties = new CreateSessionRequestModel
            {
                AccessIdentityGuid = connectionToken,
                NationalPracticeCode = odsCode
            };

            return new Request()
                .ConfigurePath(PathSessions)
                .ConfigureMethod(Methods.Post)
                .ConfigureEndUserSessionId(endUserSessionId)
                .ConfigureApplicationHeader()
                .ConfigureVersionHeader()
                .ConfigureHeader("Content-Type", "application/json; charset=UTF-8")
                .ConfigureBody(JsonConvert.SerializeObject(bodyProperties));
        }

        public Mapping RespondWithSuccess(string sessionId, string title, string firstName, string surname, string userPatientLinkToken, string odsCode, AssociationType associationType)
        {
            var responseBody = new SessionsResponse(sessionId, title, firstName, surname, userPatientLinkToken, odsCode,
                associationType);

            var response = new Response()
                .ConfigureStatusCode(HttpStatusCode.OK)
                .ConfigureBodyObject(responseBody);

            return new Mapping(_request, response);
        }
        
        public Mapping RespondWithUserNotRegistered()
        {
            return RespondWithException(-1030,
                "User Identity '00000000-0000-0000-0000-000000000000' required account status 'Inactive, Active' from Application '00000000-0000-0000-0000-000000000000'. Actual account status is 'NotRegistered'. Extra info: Invalid account registration status");
        }

        private Mapping RespondWithException(int internalResponseCode, string message)
        {

            var responseBody = new ExceptionResponse(internalResponseCode, message);

            var response = new Response()
                .ConfigureStatusCode(HttpStatusCode.InternalServerError)
                .ConfigureBodyObject(responseBody);

            return new Mapping(_request, response);
        }

        public Mapping RespondWithServerError()
        {
            var response = new Response()
                .ConfigureStatusCode(HttpStatusCode.InternalServerError);

            return new Mapping(_request, response);
        }

        public Mapping RespondWithForbidden()
        {
            return RespondWithForbiddenException(-1030,
                "Exception occurred during API processing.");
        }

        private Mapping RespondWithForbiddenException(int internalResponseCode, string message)
        {

            var responseBody = new ExceptionResponse(internalResponseCode, message);

            var response = new Response()
                .ConfigureStatusCode(HttpStatusCode.Forbidden)
                .ConfigureBodyObject(responseBody);

            return new Mapping(_request, response);
        }

        public Mapping RespondWithBadGateway()
        {
            var response = new Response()
                .ConfigureStatusCode(HttpStatusCode.BadGateway);

            return new Mapping(_request, response);
        }
    }
}
