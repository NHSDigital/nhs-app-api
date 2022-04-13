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
using NHSOnline.Backend.Messages.Areas.Messages;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.Messages
{
    public class MessagesController : Controller
    {
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly IMessageService _messageService;
        private readonly ILogger<MessagesController> _logger;
        private readonly IMetricLogger _metricLogger;
        private readonly IEventHubLogger _eventHubLogger;

        private readonly IMapper<SenderContext, SenderContextEventLogData>
            _messageSenderContextEventLogDataMapper;

        public MessagesController
        (
            IMessageService messageService,
            ILogger<MessagesController> logger,
            IMetricLogger metricLogger,
            IEventHubLogger eventHubLogger,
            IMapper<SenderContext, SenderContextEventLogData> messageSenderContextEventLogDataMapper,
            IAccessTokenProvider accessTokenProvider)
        {
            _messageService = messageService;
            _logger = logger;
            _metricLogger = metricLogger;
            _eventHubLogger = eventHubLogger;
            _messageSenderContextEventLogDataMapper = messageSenderContextEventLogDataMapper;
            _accessTokenProvider = accessTokenProvider;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme)]
        [ApiVersionRoute("api/{nhsLoginId}/messages")]
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

                await messageResult.Accept(
                    new AddMessageLogResultVisitor(_eventHubLogger, _messageSenderContextEventLogDataMapper, _logger));
                return messageResult.Accept(new AddMessageResultVisitor());
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
        [ApiVersionRoute("api/me/messages")]
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

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ApiVersionRoute("api/me/messages/{messageId}")]
        public async Task<IActionResult> GetMessage([FromRoute] string messageId)
        {
            try
            {
                _logger.LogEnter();

                var result = await _messageService.GetMessage(_accessTokenProvider.AccessToken, messageId);

                return result.Accept(new MessageResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get message with exception");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ApiVersionRoute("api/me/messages/senders")]
        public async Task<IActionResult> GetSenders()
        {
            try
            {
                _logger.LogEnter();

                var result = await _messageService.GetSenders(_accessTokenProvider.AccessToken);

                return result.Accept(new SendersResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get senders with exception");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPatch]
        [Authorize(AuthenticationSchemes = Constants.AuthenticationSchemeGroupings.JwtAndCookieAuthenticationScheme)]
        [ApiVersionRoute("api/me/messages/{messageId}")]
        public async Task<IActionResult> Patch([FromBody] JsonPatchDocument<Message> patchDocument, string messageId)
        {
            try
            {
                _logger.LogEnter();

                var messageResult =
                    await _messageService.UpdateMessage(patchDocument, _accessTokenProvider.AccessToken, messageId);

                await messageResult.Accept(
                    new MessageLogPatchResultVisitor(_metricLogger, _eventHubLogger,
                        _messageSenderContextEventLogDataMapper, _logger));
                return await messageResult.Accept(new MessagePatchResultVisitor());
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
