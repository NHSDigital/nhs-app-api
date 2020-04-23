using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity.Models;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.AssertedLoginIdentity
{
    public class CreateJwtResultVisitor : ICreateJwtResultVisitor<IActionResult>
    {

        private readonly ILogger _logger;
        private readonly CreateJwtRequest _request;

        public CreateJwtResultVisitor(ILogger logger,
            CreateJwtRequest request)
        {
            _logger = logger;
            _request = request;
        }
        public IActionResult Visit(CreateJwtResult.Success result)
        {

            var kvp = new Dictionary<string, string>
            {
                { "ProviderId", _request.ProviderId },
                { "ProviderName", _request.ProviderName },
                { "JumpOffId", _request.JumpOffId },
                { "IntendedRelyingPartyUrl", _request.IntendedRelyingPartyUrl },
            };
            _logger.LogInformationKeyValuePairs("Created Asserted Login Identity", kvp);

            return new CreatedResult(string.Empty, result.Response);
        }

        public IActionResult Visit(CreateJwtResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}