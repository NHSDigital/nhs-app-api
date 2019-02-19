using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;
using NHSOnline.Backend.Worker.OrganDonation.ApiModels;
using NHSOnline.Backend.Worker.OrganDonation.Models;

namespace NHSOnline.Backend.Worker.OrganDonation.Mappers
{
    internal class OrganDonationRegistrationResponseMapper : IMapper<OrganDonationResponse<RegistrationResponse>,
        OrganDonationRegistrationResponse>
    {
        private readonly ILogger<OrganDonationRegistrationResponseMapper> _logger;
        private readonly string[] _registerConflictErrorCodes = { "10001", "10002" };
        private readonly string[] _updateConflictErrorCodes = { "10201", "10202" };

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

            var errorCode = source.Body?.Issue?.FirstOrDefault().Details?.Coding?.FirstOrDefault()?.Code;

            var isConflicted = _registerConflictErrorCodes.Contains(errorCode) || _updateConflictErrorCodes.Contains(errorCode);
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