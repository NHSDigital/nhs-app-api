using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Auth.AspNet.ApiKey;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Models;
using NHSOnline.Backend.UserInfoApi.Areas.UserResearch;
using NHSOnline.Backend.UserInfoApi.Areas.UserResearch.Models;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserInfo
{
    public class InfoController : Controller
    {
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly IInfoService _infoService;
        private readonly IUserResearchService _userResearchService;
        private readonly IMapper<UserProfile, InfoUserProfile> _userProfileMapper;
        private readonly ILogger<InfoController> _logger;
        private readonly IAuditor _auditor;
        private readonly IMetricLogger _metricLogger;

        public InfoController
        (
            IAccessTokenProvider accessTokenProvider,
            IInfoService infoService,
            IUserResearchService userResearchService,
            IMapper<UserProfile, InfoUserProfile> userProfileMapper,
            ILogger<InfoController> logger,
            IAuditor auditor,
            IMetricLogger metricLogger)
        {
            _accessTokenProvider = accessTokenProvider;
            _infoService = infoService;
            _userResearchService = userResearchService;
            _userProfileMapper = userProfileMapper;
            _logger = logger;
            _auditor = auditor;
            _metricLogger = metricLogger;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("api/me/info")]
        public async Task<IActionResult> Post([UserProfile] UserProfile userProfile)
        {
            try
            {
                _logger.LogEnter();

                var userInfo = _userProfileMapper.Map(userProfile);
                var result = await _infoService.Send(_accessTokenProvider.AccessToken, userInfo);

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
                var result = await _infoService.GetInfo(_accessTokenProvider.AccessToken);
                return result.Accept(new GetMeInfoResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError(e,"Failed to get info with exception");
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

        [HttpPost]
        [Route("api/me/info/userresearch")]
        public async Task<IActionResult> PostUserResearchPreference([FromBody] UserResearchRequest userResearchRequest,
            [UserProfile] UserProfile userProfile)
        {
            try
            {
                _logger.LogEnter();

                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                if (userResearchRequest.Preference == UserResearchPreference.OptOut)
                {
                    await _metricLogger.UserResearchOptOut();
                    return new NoContentResult();
                }

                var accessToken = _accessTokenProvider.AccessToken;
                var userInfo = _userProfileMapper.Map(userProfile);
                var result = await _auditor.Audit()
                    .AccessToken(accessToken)
                    .NhsNumber(userProfile.NhsNumber)
                    .Supplier(Supplier.Qualtrics)
                    .Operation(AuditingOperations.UserResearchPreferencePost)
                    .Details("Attempting to create Session")
                    .Execute(async () => await _userResearchService.Post(userInfo,
                        _accessTokenProvider.AccessToken));

                return result.Accept(new PostUserResearchResultVisitor(_metricLogger));
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to patch info with exception: {e}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}