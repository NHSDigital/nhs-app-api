using System;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public interface ICitizenIdService
    {
        Task<GetUserProfileResult> GetUserProfile(string authCode, string codeVerifier, Uri redirectUrl);

        Task<GetUserProfileResult> GetUserProfile(string accessToken);
    }
}