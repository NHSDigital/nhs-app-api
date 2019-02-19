using System;
using System.Globalization;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.OrganDonation.ApiModels;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;
using static NHSOnline.Backend.Support.Constants.OrganDonationConstants;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal class RegistrationLookupRequestMapper : IMapper<OrganDonationRegistration, RegistrationLookupRequest>
    {
        private readonly ILogger<RegistrationLookupRequestMapper> _logger;

        public RegistrationLookupRequestMapper(ILogger<RegistrationLookupRequestMapper> logger)
        {
            _logger = logger;
        }
        
        public RegistrationLookupRequest Map(OrganDonationRegistration source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ThrowError)
                .IsValid();
            
            return new RegistrationLookupRequest
            {
                NhsNumber =  source.NhsNumber.Replace(" ", string.Empty, StringComparison.InvariantCulture),
                BirthDate = source.DateOfBirth?.ToString(DateFormat, CultureInfo.InvariantCulture),
                Family = source.Name?.Surname ?? string.Empty,
                Given = source.Name?.GivenName ?? string.Empty
            };
        }
    }
}