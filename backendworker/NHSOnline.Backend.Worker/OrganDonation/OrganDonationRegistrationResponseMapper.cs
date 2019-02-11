using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.OrganDonation.ApiModels;
using NHSOnline.Backend.Worker.Support;
using static NHSOnline.Backend.Worker.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal class OrganDonationRegistrationResponseMapper : IMapper<OrganDonationResponse<RegistrationResponse>,
        OrganDonationRegistrationResponse>
    {
        private readonly ILogger<OrganDonationRegistrationResponseMapper> _logger;
        private readonly string[] _inProgressSubmitErrorCodes = { "10001", "10002" };

        public OrganDonationRegistrationResponseMapper(ILogger<OrganDonationRegistrationResponseMapper> logger)
        {
            _logger = logger;
        }

        public OrganDonationRegistrationResponse Map(OrganDonationResponse<RegistrationResponse> source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ThrowError)
                .IsNotNull(source?.Body, nameof(source.Body), ThrowError)
                .IsValid();

            var errorCode = source.Body?.Issue?.Details?.Coding?.FirstOrDefault()?.Code;

            var isConflicted = _inProgressSubmitErrorCodes.Contains(errorCode);
            if (isConflicted)
            {
                _logger.LogInformation($"Registration in conflict. {source.Body?.Issue} ");
            }

            return new OrganDonationRegistrationResponse
            {
                Identifier = source.Body.Id,
                State = isConflicted ? State.Conflicted : State.Ok
            };
        }
    }
}