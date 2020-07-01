using System.Net;

namespace NHSOnline.App.Api.Client.Session
{
    public sealed class ApiCreateSessionResponse
    {
        private readonly UserSessionResponseModel _responseModel;
        private readonly CookieContainer _cookies;

        internal ApiCreateSessionResponse(UserSessionResponseModel responseModel, CookieContainer cookies)
        {
            _responseModel = responseModel;
            _cookies = cookies;
        }
    }
}