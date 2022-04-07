using System.Threading.Tasks;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UserInfo.Areas.UserInfo.Models;
using NHSOnline.Backend.UserInfo.Areas.UserResearch;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserResearch
{
    public interface IUserResearchService
    {
        Task<PostUserResearchResult> Post(InfoUserProfile userProfile, AccessToken accessToken);
    }
}