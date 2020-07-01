using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Api.Session
{
    public interface ISessionService
    {
        Task<CreateSessionResult> CreateSession(string authCode, string codeVerifier, Uri redirectUrl);
    }
}
