using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Models;
using NHSOnline.Backend.UserInfoApi.Clients;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserResearch
{
    internal class UserResearchService : IUserResearchService
    {
        private readonly ILogger<InfoController> _logger;
        private readonly IUserResearchClient _userResearchClient;

        public UserResearchService
        (
            ILogger<InfoController> logger,
            IUserResearchClient userResearchClient
        )
        {
            _logger = logger;
            _userResearchClient = userResearchClient;
        }

        public async Task<PostUserResearchResult> Post(
            InfoUserProfile userProfile,
            AccessToken accessToken)
        {
            try
            {
                var email = userProfile.Email;
                var odsCode = userProfile.OdsCode;
                var nhsLoginId = accessToken.Subject;
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogError("No Email was found when posting to User Research");
                    return new PostUserResearchResult.EmailMissing();
                }
                if (odsCode == null)
                {
                    _logger.LogError("No ODSCode was found when posting to User Research");
                }
                return await Post(nhsLoginId, email, odsCode);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to post User Research with exception: {e}");
                return new PostUserResearchResult.InternalServerError();
            }
        }

        private async Task<PostUserResearchResult> Post(string nhsLoginId, string email, string odsCode)
        {
            _logger.LogEnter();

            try
            {
                var result = await _userResearchClient.Post(nhsLoginId, email, odsCode);
                if (result.HasSuccessResponse)
                {
                    _logger.LogInformation("User Research post succeeded");
                    return new PostUserResearchResult.Success();
                }
                _logger.LogError($"User Research post has failed with error status {result.StatusCode}");
                return new PostUserResearchResult.Failure();
            }
            catch (Exception e)
            {
                _logger.LogError($"User Research post has failed with exception: {e}");
                return new PostUserResearchResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}