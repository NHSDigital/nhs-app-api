using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public interface ICitizenIdService
    {
        [SuppressMessage("Microsoft.Design", "CA1054", Justification = "Uris are not serializable")]
        Task<GetUserProfileResult> GetUserProfile(string authCode, string codeVerifier, string redirectUrl);

        Task<GetUserProfileResult> GetUserProfile(string accessToken);
    }
}