using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    internal class OrganDonationIdentifierMapper : IOrganDonationIdentifierMapper
    {
        private readonly ILogger<OrganDonationIdentifierMapper> _logger;

        public OrganDonationIdentifierMapper(ILogger<OrganDonationIdentifierMapper> logger)
        {
            _logger = logger;
        }

        public Identifier Map(string source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ValidateAndLog.ValidationOptions.ThrowError)
                .IsValid();

            return new Identifier
            {
                System = Constants.OrganDonationConstants.IdentifierSystem,
                Value = source.RemoveWhiteSpace()
            };
        }
    }
}
