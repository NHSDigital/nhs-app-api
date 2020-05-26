using System.Threading.Tasks;

namespace NHSOnline.Backend.UserInfoApi.Clients
{
    internal interface IUserResearchClient
    {
        Task<UserResearchClientResponse> Post(string nhsLoginId, string email, string odsCode);
    }
}