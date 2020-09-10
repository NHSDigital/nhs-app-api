using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Auth.AspNet.ApiKey;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public class MessagesController : Controller
    {
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly IMessageService _messageService;
        private readonly ILogger<MessagesController> _logger;
        private readonly IMetricLogger _metricLogger;

        public MessagesController
        (
            IMessageService messageService,
            ILogger<MessagesController> logger,
            IMetricLogger metricLogger,
            IAccessTokenProvider accessTokenProvider)
        {
            _messageService = messageService;
            _logger = logger;
            _metricLogger = metricLogger;
            _accessTokenProvider = accessTokenProvider;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme)]
        [Route("api/{nhsLoginId}/messages")]
        public async Task<IActionResult> Post
        (
            [FromBody] AddMessageRequest addMessageRequest,
            [FromRoute(Name = "nhsLoginId")] string nhsLoginId
        )
        {
            try
            {
                _logger.LogEnter();

                var messageResult =
                    await _messageService.Send(addMessageRequest, nhsLoginId);

                return messageResult.Accept(new MessageResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to post message with exception");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("api/me/messages")]
        public async Task<IActionResult> Get([FromQuery] string sender = null, [FromQuery] bool summary = false)
        {
            try
            {
                _logger.LogEnter();

                var messagesResult = await GetUserMessages(sender, summary, _accessTokenProvider.AccessToken);

                return messagesResult.Accept(new MessagesResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get messages with exception");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPatch]
        [UserProfile]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("api/me/messages/{messageId}")]
        public async Task<IActionResult> Patch([FromBody] JsonPatchDocument<Message> patchDocument, string messageId)
        {
            try
            {
                _logger.LogEnter();

                var messageResult = await _messageService.UpdateMessage(patchDocument,  _accessTokenProvider.AccessToken, messageId);

                return await messageResult.Accept(new MessagePatchResultVisitor(_metricLogger));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to update message with exception");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<MessagesResult> GetUserMessages(string sender, bool summary, AccessToken accessToken)
        {
            var hasSender = !string.IsNullOrWhiteSpace(sender);
            if (!hasSender && !summary || hasSender && summary)
            {
                _logger.LogError("Either `sender` or `summary` is required, but cannot be both");
                return new MessagesResult.BadRequest();
            }

            return hasSender
                ? await _messageService.GetMessages(accessToken, sender)
                : await _messageService.GetSummaryMessages(accessToken);
        }
    }
}