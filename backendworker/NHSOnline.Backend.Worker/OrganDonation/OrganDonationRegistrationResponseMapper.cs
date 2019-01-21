using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;
using static NHSOnline.Backend.Worker.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal class OrganDonationRegistrationResponseMapper : IMapper<OrganDonationResponse<RegistrationResponse>,
        OrganDonationRegistrationResponse>
    {
        private readonly ILogger<OrganDonationRegistrationResponseMapper> _logger;

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

            return new OrganDonationRegistrationResponse
            {
                Identifier = source.Body.Id
            };
        }
    }
}