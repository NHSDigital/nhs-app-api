using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Auth.CitizenId.Models;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public interface ICitizenIdClient
    {
        Task<CitizenIdApiObjectResponse<Token>> ExchangeAuthToken(string authCode, string codeVerifier, Uri redirectUrl);
        Task<CitizenIdApiObjectResponse<JsonWebKeySet>> GetSigningKeys();
        Task<CitizenIdApiObjectResponse<UserInfo>> GetUserInfo(string accessToken);
        Task<CitizenIdApiObjectResponse<Token>> RefreshAccessToken(string refreshToken);
    }
}