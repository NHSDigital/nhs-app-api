using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Auth.CitizenId.Models;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public interface ICitizenIdService
    {
        Task<GetUserProfileResult> GetUserProfile(string authCode, string codeVerifier, Uri redirectUrl);

        Task<GetUserProfileResult> GetUserProfile(string accessToken);

        Task<RefreshAccessTokenResult> RefreshAccessToken(string refreshToken);
    }
}