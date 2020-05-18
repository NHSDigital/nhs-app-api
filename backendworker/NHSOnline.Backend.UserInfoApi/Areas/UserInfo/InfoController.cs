using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Auth.AspNet.ApiKey;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Models;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserInfo
{
    public class InfoController : Controller
    {
        private readonly IInfoService _infoService;
        private readonly ICitizenIdService _citizenIdService;
        private readonly IMapper<UserProfile, InfoUserProfile> _userProfileMapper;
        private readonly ILogger<InfoController> _logger;

        public InfoController
        (
            IInfoService infoService,
            ICitizenIdService citizenIdService,
            IMapper<UserProfile, InfoUserProfile> userProfileMapper,
            ILogger<InfoController> logger
        )
        {
            _infoService = infoService;
            _citizenIdService = citizenIdService;
            _userProfileMapper = userProfileMapper;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("api/me/info")]
        public async Task<IActionResult> Post()
        {
            try
            {
                _logger.LogEnter();

                var accessToken = HttpContext.GetAccessToken(_logger);

                var userProfile = await GetUserProfile(accessToken);

                if (userProfile == null)
                {
                    return new StatusCodeResult(StatusCodes.Status502BadGateway);
                }
                var result = await _infoService.Send(accessToken, userProfile);

                return result.Accept(new PostInfoResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to post user info with exception: {e}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("api/me/info")]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogEnter();

                var accessToken = HttpContext.GetAccessToken(_logger);

                var result = await _infoService.GetInfo(accessToken);

                return result.Accept(new GetMeInfoResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get info with exception: {e}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme)]
        [Route("api/info")]
        public async Task<IActionResult> Get([FromQuery] string odsCode, [FromQuery] string nhsNumber)
        {
            try
            {
                if (!string.IsNullOrEmpty(odsCode) && !string.IsNullOrEmpty(nhsNumber)
                    || string.IsNullOrEmpty(odsCode) && string.IsNullOrEmpty(nhsNumber))
                {
                    _logger.LogError("Expected either odsCode or nhsNumber to be supplied");
                    return BadRequest();
                }

                _logger.LogEnter();

                var result =
                    odsCode != null
                        ? await _infoService.GetInfoByOdsCode(odsCode)
                        : await _infoService.GetInfoByNhsNumber(nhsNumber);

                return result.Accept(new GetInfoResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get user info with exception: {e}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<InfoUserProfile> GetUserProfile(AccessToken accessToken)
        {
            var userProfileResult = await _citizenIdService.GetUserProfile(accessToken.ToString());
            var cidUserProfileOption = userProfileResult.UserProfile;

            if (!cidUserProfileOption.HasValue)
            {
                _logger.LogError("No CID profile was found for access code");
                return null;
            }

            return _userProfileMapper.Map(cidUserProfileOption.ValueOrFailure());
        }
    }
}
