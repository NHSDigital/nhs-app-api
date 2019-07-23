using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Auth.CitizenId.Models;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public interface ICitizenIdClient
    {
        [SuppressMessage("Microsoft.Design", "CA1054", Justification = "Uris are not serializable")]
        Task<CitizenIdApiObjectResponse<Token>> ExchangeAuthToken(string authCode, string codeVerifier,
            string redirectUrl);
        Task<CitizenIdApiObjectResponse<JsonWebKeySet>> GetSigningKeys();
        Task<CitizenIdApiObjectResponse<UserInfo>> GetUserInfo(string accessToken);
    }
}