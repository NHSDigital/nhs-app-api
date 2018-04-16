using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public interface ICitizenIdService
    {
        Task<Option<UserProfile>> GetUserProfile(string authCode, string codeVerifier, string clientId,
            string clientSecret);
    }

    public class CitizenIdService : ICitizenIdService
    {
        // TODO - NHSO-454
        public Task<Option<UserProfile>> GetUserProfile(string authCode, string codeVerifier, string clientId, string clientSecret)
        {
            throw new System.NotImplementedException();
        }
    }
}
