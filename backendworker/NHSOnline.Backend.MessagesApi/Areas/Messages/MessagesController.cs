using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public class MessagesController : Controller
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<MessagesController> _logger;
        private readonly IAuditor _auditor;

        public MessagesController
        (
            IMessageService messageService,
            ILogger<MessagesController> logger,
            IAuditor auditor
        )
        {
            _messageService = messageService;
            _logger = logger;
            _auditor = auditor;
        }

        [HttpPost]
        [AllowAnonymous]
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

                if (!IsMessageRequestValid(addMessageRequest, nhsLoginId))
                {
                    return BadRequest();
                }

                var messageResult =
                    await _messageService.Send(addMessageRequest, nhsLoginId);

                return messageResult.Accept(new MessageResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to post message with exception: {e}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpGet]
        [Route("api/me/messages")]
        public async Task<IActionResult> Get([FromQuery] string sender = null, [FromQuery] bool summary = false)
        {
            try
            {
                _logger.LogEnter();

                var hasSender = !string.IsNullOrWhiteSpace(sender);
                if (!hasSender && !summary || hasSender && summary)
                {
                    _logger.LogError("Either `sender` or `summary` is required, but cannot be both");
                    return BadRequest();
                }

                var accessToken = HttpContext.GetAccessToken(_logger);
                
                await _auditor.AuditSecureTokenEvent(accessToken, Supplier.Microsoft,
                    AuditingOperations.GetUserMessagesAuditTypeRequest, "Attempting to get users messages");

                var messageResult = hasSender
                    ? await GetMessages(accessToken, sender)
                    : await GetSummaryMessages(accessToken);

                await messageResult.Accept(new MessagesAuditingVisitor(_logger, _auditor, accessToken));
                return messageResult.Accept(new MessagesResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get messages with exception: {e}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<MessagesResult> GetSummaryMessages(AccessToken accessToken)
        {
            await AuditGetMessages(accessToken, "Attempting to get users summary messages");

            return await _messageService.GetSummaryMessages(accessToken);
        }

        private async Task<MessagesResult> GetMessages(AccessToken accessToken, string sender)
        {
            await AuditGetMessages(accessToken, $"Attempting to get users messages from '{sender}'");

            return await _messageService.GetMessages(accessToken, sender);
        }

        private async Task AuditGetMessages(AccessToken accessToken, string details)
            => await _auditor.AuditSecureTokenEvent(accessToken, Supplier.Microsoft,
                AuditingOperations.GetUserMessagesAuditTypeRequest, details);

        private bool IsMessageRequestValid(AddMessageRequest addMessageRequest, string nhsLoginId)
        {
            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(addMessageRequest?.Sender, nameof(addMessageRequest.Sender))
                .IsNotNullOrWhitespace(addMessageRequest?.Body, nameof(addMessageRequest.Body))
                .IsNotNullOrWhitespace(nhsLoginId, nameof(nhsLoginId))
                .IsValid();
        }
    }
}