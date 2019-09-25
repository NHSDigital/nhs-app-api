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

        public MessagesController(
            IMessageService messageService,
            ILogger<MessagesController> logger,
            IAuditor auditor)
        {
            _messageService = messageService;
            _logger = logger;
            _auditor = auditor;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/{nhsLoginId}/messages")]
        public async Task<IActionResult> Post([FromBody] AddMessageRequest addMessageRequest, [FromRoute(Name = "nhsLoginId")] string nhsLoginId)
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
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogEnter();

                var accessToken = GetAccessToken();
                
                await _auditor.AuditSecureTokenEvent(accessToken, Supplier.Microsoft,
                    AuditingOperations.GetUserMessagesAuditTypeRequest, "Attempting to get users messages");

                var messageResult = await _messageService.GetMessages(accessToken);

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

        private bool IsMessageRequestValid(AddMessageRequest addMessageRequest, string nhsLoginId)
        {
            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(addMessageRequest?.Sender, nameof(addMessageRequest.Sender))
                .IsNotNullOrWhitespace(addMessageRequest?.Body, nameof(addMessageRequest.Body))
                .IsNotNullOrWhitespace(nhsLoginId, nameof(nhsLoginId))
                .IsValid();
        }

        private AccessToken GetAccessToken()
        {
            return HttpContext.GetAccessToken(_logger);
        }
    }
}