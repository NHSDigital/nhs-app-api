using System.Net;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.Mocking.Models;
using NHSOnline.Backend.Worker.Mocking.Shared;

namespace NHSOnline.Backend.Worker.Mocking.CID
{
    public class UserProfileConfigurator
    {
        private const string PathToken = "/userinfo";
        private const string BearerToken = "Bearer";

        private readonly Request _request;
        private readonly Response _response;

        private UserProfileConfigurator(Request request)
        {
            _request = request;
            _response = new Response();
        }

        public static UserProfileConfigurator ForRequest(string bearerToken)
        {
            return new UserProfileConfigurator(CreateUserProfileRequest(bearerToken));
        }

        private static Request CreateUserProfileRequest(
            string bearerToken)
        {
            return new Request()
                .ConfigurePath(PathToken)
                .ConfigureMethod(Methods.Get)
                .ConfigureUserProfileAuthorizationHeader(BearerToken);
        }

        public Mapping RespondWithSuccess(string odsCode, string connectionToken)
        {
            var responseBody = new UserInfoResponse(odsCode, connectionToken);

            _response
                .ConfigureStatusCode(HttpStatusCode.OK)
                .ConfigureBodyObject(responseBody);

            return new Mapping(_request, _response);
        }

        public Mapping RespondWithServiceUnavailable()
        {
            const string responseBody = "Service Unavailable";

            _response
                .ConfigureStatusCode(HttpStatusCode.ServiceUnavailable)
                .ConfigureBody(responseBody);

            return new Mapping(_request, _response);
        }

        public Mapping RespondWithServerError()
        {
            const string responseBody = "Server Error";

            _response
                .ConfigureStatusCode(HttpStatusCode.InternalServerError)
                .ConfigureBody(responseBody);

            return new Mapping(_request, _response);
        }
    }
}