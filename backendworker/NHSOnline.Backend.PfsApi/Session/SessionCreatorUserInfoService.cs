using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UserInfo.Areas.UserInfo;
using NHSOnline.Backend.UserInfo.Areas.UserInfo.Models;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class SessionCreatorUserInfoService
    {
        private readonly IInfoService _infoService;
        private readonly IMapper<CitizenIdSessionResult, InfoUserProfile> _userProfileMapper;
        private readonly ILogger<SessionCreatorUserInfoService> _logger;
        public SessionCreatorUserInfoService(
            IInfoService infoService,
            ILogger<SessionCreatorUserInfoService> logger,
            IMapper<CitizenIdSessionResult, InfoUserProfile> userProfileMapper
        )
        {
            _infoService = infoService;
           _logger = logger;
           _userProfileMapper = userProfileMapper;
        }


        internal async Task Update(
            ServiceJourneyRulesResponse serviceJourneyRules,
            CitizenIdSessionResult citizenIdSessionResult)
        {
            if (serviceJourneyRules.Journeys.UserInfo == true)
            {
                var userInfoProfile = _userProfileMapper.Map(citizenIdSessionResult);
                await _infoService.Send(AccessToken.Parse(_logger, citizenIdSessionResult.Session.AccessToken), userInfoProfile);
            }
        }
    }
}