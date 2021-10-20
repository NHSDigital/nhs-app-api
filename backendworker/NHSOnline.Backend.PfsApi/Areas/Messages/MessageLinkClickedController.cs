using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.Messages
{
    public class MessageLinkClickedController : Controller
    {
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly ILogger<MessageLinkClickedController> _logger;
        private readonly IMessageLinkClickedService _messageLinkClickedService;

        public MessageLinkClickedController(
            IAccessTokenProvider accessTokenProvider,
            ILogger<MessageLinkClickedController> logger,
            IMessageLinkClickedService messageLinkClickedService
        )
        {
            _accessTokenProvider = accessTokenProvider;
            _logger = logger;
            _messageLinkClickedService = messageLinkClickedService;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = Constants.AuthenticationSchemeGroupings.JwtAndCookieAuthenticationScheme)]
        [ApiVersionRoute("api/me/messages/{messageId}/link-clicked/metrics")]
        public async Task<IActionResult> Post
        (
            [FromRoute(Name = "messageId")] string messageId,
            [FromBody] MessageLinkClickedRequest request
        )
        {
            try
            {
                _logger.LogEnter();

                var result = await _messageLinkClickedService.LogLinkClicked(
                    _accessTokenProvider?.AccessToken?.Subject,
                    new MessageLink
                    {
                        MessageId = messageId,
                        Link = request?.Link
                    }
                );

                return result.Accept(new MessageLinkClickedResultVisitor());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to record metrics for message {messageId}, request {request}");

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
