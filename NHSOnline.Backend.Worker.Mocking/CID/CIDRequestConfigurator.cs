using NHSOnline.Backend.Worker.Mocking.Models;

namespace NHSOnline.Backend.Worker.Mocking.CID
{
    public static class CIDRequestConfigurator
    {
        private const string HeaderAuthorization = "Authorization";

        public static Request ConfigureTokensAuthorizationHeader(this Request request, string authorization = null)
        {
            request.ConfigureAuthorizationHeader(HeaderAuthorization, authorization);

            return request;
        }

        public static Request ConfigureUserProfileAuthorizationHeader(this Request request, string authorization = null)
        {
            request.ConfigureAuthorizationHeader(HeaderAuthorization, authorization);

            return request;
        }
    }
}
