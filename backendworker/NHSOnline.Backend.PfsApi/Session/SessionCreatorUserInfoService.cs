using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.PfsApi.UserInfo;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class SessionCreatorUserInfoService
    {
        private readonly IUserInfoService _userInfoService;

        public SessionCreatorUserInfoService(IUserInfoService userInfoService)
            => _userInfoService = userInfoService;

        internal async Task Update(
            ICreateSessionRequest request,
            ServiceJourneyRulesResponse serviceJourneyRules,
            CitizenIdSessionResult citizenIdSessionResult)
        {
            if (serviceJourneyRules.Journeys.UserInfo == true)
            {
                await _userInfoService.Update(citizenIdSessionResult.Session.AccessToken, request.HttpContext);
            }
        }
    }
}