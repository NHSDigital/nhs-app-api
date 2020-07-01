using NHSOnline.App.Api.Client.Session;

namespace NHSOnline.App.Api.Session
{
    public sealed class UserSession
    {
        private readonly ApiCreateSessionResponse _createSessionResponse;

        public UserSession(ApiCreateSessionResponse createSessionResponse)
        {
            _createSessionResponse = createSessionResponse;
        }
    }
}