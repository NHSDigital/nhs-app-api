using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.UserInfo
{
    public class UserInfoService: IUserInfoService
    {
        private readonly ILogger<UserInfoService> _logger;
        private readonly IUserInfoClient _userInfoClient;
        
        public UserInfoService(
            ILogger<UserInfoService> logger,
            IUserInfoClient userInfoClient)
        {
            _logger = logger;
            _userInfoClient = userInfoClient;
        }

        public async Task<UserInfoResult> Update(string accessToken)
        {
            _logger.LogEnter();
            try
            {
                var response = await _userInfoClient.Post(accessToken);

                if (response.HasSuccessResponse)
                {
                    return new UserInfoResult.Success();
                }
                return new UserInfoResult.BadGateway();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"User Info API failed with exception, {e}");
                return new UserInfoResult.BadGateway();
            } 
            catch (Exception e)
            {
                _logger.LogError($"User Info API failed with exception: {e}");
                return new UserInfoResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}