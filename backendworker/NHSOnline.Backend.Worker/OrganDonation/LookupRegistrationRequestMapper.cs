using System;
using System.Globalization;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public class LookupRegistrationRequestMapper : IMapper<OrganDonationRegistration, LookupRegistrationRequest>
    {
        public LookupRegistrationRequest Map(OrganDonationRegistration source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            
            return new LookupRegistrationRequest
            {
                NhsNumber =  source.NhsNumber.Replace(" ", string.Empty, StringComparison.InvariantCulture),
                BirthDate = source.DateOfBirth?.ToString("yyyy-MM-dd", CultureInfo.DefaultThreadCurrentCulture),
                Family = source.Name?.Surname ?? string.Empty,
                Given = source.Name?.GivenName ?? string.Empty
            };
        }
    }
}