using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.AspNet.ApiKey;
using NHSOnline.Backend.Messages.Areas.Messages;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.Messages
{
    public class SenderController : Controller
    {
        private readonly ILogger<SenderController> _logger;
        private readonly ISenderService _senderService;

        public SenderController
        (
            ILogger<SenderController> logger,
            ISenderService senderService)
        {
            _logger = logger;
            _senderService = senderService;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme)]
        [ApiVersionRoute("api/users/senders")]
        public async Task<IActionResult> Post
        (
            [FromBody] Sender sender
        )
        {
            try
            {
                _logger.LogEnter();

                if (!IsSenderPostRequestValid(sender))
                {
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
                }

                var senderResult = await _senderService.Create(sender);

                return senderResult.Accept(new SenderPostResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to post sender with exception");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme)]
        [ApiVersionRoute("api/users/senders/{senderId}")]
        public async Task<IActionResult> GetSender([FromRoute] string senderId)
        {
            try
            {
                _logger.LogEnter();

                if (string.IsNullOrEmpty(senderId?.Trim()))
                {
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
                }

                var result = await _senderService.GetSender(senderId);

                return result.Accept(new SenderResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get senderId with exception");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private bool IsSenderPostRequestValid(Sender addSenderRequest)
        {
            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(addSenderRequest?.Id, nameof(addSenderRequest.Id))
                .IsNotNullOrWhitespace(addSenderRequest?.Name, nameof(addSenderRequest.Name))
                .IsValid();
        }
    }
}