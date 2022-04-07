using System.Threading.Tasks;

namespace NHSOnline.Backend.UserInfo.Clients
{
    public interface IUserResearchClient
    {
        Task<UserResearchClientResponse> Post(string nhsLoginId, string email, string odsCode);
    }
}