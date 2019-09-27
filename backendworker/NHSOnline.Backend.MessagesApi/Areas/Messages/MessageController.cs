using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<MessageController> _logger;

        public MessageController(
            IMessageService messageService,
            ILogger<MessageController> logger)
        {
            _messageService = messageService;
            _logger = logger;
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