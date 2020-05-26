using System.Threading.Tasks;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Models;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserResearch
{
    public interface IUserResearchService
    {
        Task<PostUserResearchResult> Post(InfoUserProfile userProfile, AccessToken accessToken);
    }
}