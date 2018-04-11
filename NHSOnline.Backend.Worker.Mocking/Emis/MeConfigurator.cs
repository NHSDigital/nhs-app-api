using System.Net;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.Mocking.Models;
using NHSOnline.Backend.Worker.Mocking.Shared;

namespace NHSOnline.Backend.Worker.Mocking.Emis
{
    public class MeConfigurator
    {
        private const string PathMe = "/emis/me";
        private const string PathApplications = PathMe + "/applications";

        private readonly Request _request;

        private MeConfigurator(Request request)
        {
            _request = request;
        }

        public static MeConfigurator ForRequest(string endUserSessionId, string surname, string dateOfBirth, string accountId,
            string linkageKey, string nationalPracticeCode)
        {
            return new MeConfigurator(CreateApplicationsRequest(endUserSessionId, surname, dateOfBirth, accountId,
                linkageKey, nationalPracticeCode));
        }

        private static Request CreateApplicationsRequest(string endUserSessionId, string surname, string dateOfBirth, string accountId,
            string linkageKey, string nationalPracticeCode)
        {
            var requestBody =
                new LinkApplicationRequest(surname, dateOfBirth, accountId, linkageKey, nationalPracticeCode);

            return new Request()
                .ConfigurePath(PathApplications)
                .ConfigureMethod(Methods.Post)
                .ConfigureApplicationHeader()
                .ConfigureVersionHeader()
                .ConfigureEndUserSessionId(endUserSessionId)
                .ConfigureHeader("Content-Type", "application/json; charset=UTF-8")
                .ConfigureBody(JsonConvert.SerializeObject(requestBody));
        }

        public Mapping RespondWithSuccess(string accessIdentityGuid)
        {
            var responseBody = new LinkApplicationResponse(accessIdentityGuid);

            var response = new Response()
                .ConfigureStatusCode(HttpStatusCode.OK)
                .ConfigureBodyObject(responseBody);

            return new Mapping(_request, response);
        }

        public Mapping RespondWithBadRequest(string message)
        {
            var responseBody = new BadRequestResponse(message);

            var response = new Response()
                .ConfigureStatusCode(HttpStatusCode.BadRequest)
                .ConfigureBodyObject(responseBody);

            return new Mapping(_request, response);
        }

        public Mapping RespondWithBadRequest(string message, string fieldName)
        {
            var responseBody = new BadRequestResponse(message, fieldName);

            var response = new Response()
                .ConfigureStatusCode(HttpStatusCode.BadRequest)
                .ConfigureBodyObject(responseBody);

            return new Mapping(_request, response);
        }

        public Mapping RespondWithNoOnlineUserFound()
        {
            return RespondWithException(-1002, "No registered online user found for given linkage details");
        }

        public Mapping RespondWithUserAlreadyLinked()
        {
            return RespondWithException(-1002, "Registered online user is already linked");
        }

        public Mapping RespondWithInvalidLinkLevel()
        {
            return RespondWithException(-1030,
                "User Identity '00000000-0000-0000-0000-000000000000' requested access level 'Linked' from Application '00000000-0000-0000-0000-000000000000'. Actual access level is 'Restricted'. Extra info: Invalid UserApplication link level");
        }

        private Mapping RespondWithException(int internalResponseCode, string message)
        {

            var responseBody = new ExceptionResponse(internalResponseCode, message);

            var response = new Response()
                .ConfigureStatusCode(HttpStatusCode.InternalServerError)
                .ConfigureBodyObject(responseBody);

            return new Mapping(_request, response);
        }
    }
}
