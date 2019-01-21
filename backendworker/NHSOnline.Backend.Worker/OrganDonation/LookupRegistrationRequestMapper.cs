using System;
using System.Globalization;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;
using static NHSOnline.Backend.Worker.Support.ValidateAndLog.ValidationOptions;
using static NHSOnline.Backend.Worker.Constants.OrganDonationConstants;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal class LookupRegistrationRequestMapper : IMapper<OrganDonationRegistration, LookupRegistrationRequest>
    {
        private readonly ILogger<LookupRegistrationRequestMapper> _logger;

        public LookupRegistrationRequestMapper(ILogger<LookupRegistrationRequestMapper> logger)
        {
            _logger = logger;
        }
        
        public LookupRegistrationRequest Map(OrganDonationRegistration source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ThrowError)
                .IsValid();
            
            return new LookupRegistrationRequest
            {
                NhsNumber =  source.NhsNumber.Replace(" ", string.Empty, StringComparison.InvariantCulture),
                BirthDate = source.DateOfBirth?.ToString(DateFormat, CultureInfo.InvariantCulture),
                Family = source.Name?.Surname ?? string.Empty,
                Given = source.Name?.GivenName ?? string.Empty
            };
        }
    }
}