using System;
using System.Threading.Tasks;

namespace NHSOnline.Backend.PfsApi.CitizenId
{
    public interface ICitizenIdSessionService
    {
        Task<CitizenIdSessionResult> Create(string authCode, string codeVerifier, Uri redirectUrl);
    }
}