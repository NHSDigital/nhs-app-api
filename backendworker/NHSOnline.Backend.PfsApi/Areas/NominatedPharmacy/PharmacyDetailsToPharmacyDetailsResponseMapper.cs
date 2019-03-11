using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.NominatedPharmacy.Models;
using NHSOnline.Backend.Worker.GpSearch.Models;
using static NHSOnline.Backend.Worker.GpSearch.ResponseEnums;

namespace NHSOnline.Backend.Worker.Areas.NominatedPharmacy
{
    public class PharmacyDetailsToPharmacyDetailsResponseMapper : IPharmacyDetailsToPharmacyDetailsResponseMapper
    {
        private readonly ILogger<PharmacyDetailsToPharmacyDetailsResponseMapper> _logger;

        public PharmacyDetailsToPharmacyDetailsResponseMapper(ILogger<PharmacyDetailsToPharmacyDetailsResponseMapper> logger)
        {
            _logger = logger;
        }
        
        public PharmacyDetailsResponse Map(Organisation pharmacy)
        {
            if (pharmacy == null)
            {
                throw new ArgumentNullException(nameof(pharmacy));
            }

            _logger.LogInformation($"Mapping pharmacy of type {nameof(Organisation)} to { nameof(PharmacyDetailsResponse) }.");

            var pharmacyDetails = new PharmacyDetailsResponse
            {
                PharmacyName = pharmacy.OrganisationName,
                AddressLine1 = pharmacy.Address1,
                AddressLine2 = pharmacy.Address2,
                AddressLine3 = pharmacy.Address3,
                County = pharmacy.County,
                City = pharmacy.City,
                Postcode = pharmacy.Postcode,
                TelephoneNumber = pharmacy.GetContactsArray().FirstOrDefault(x => x.OrganisationContactMethodType == OrganisationContactMethodType.Telephone)?.OrganisationContactValue,
                OpeningTimes = pharmacy.GetOpeningTimesArray()
                    .Where(x => x.IsOpen && x.WeekDay.HasValue)
                    .Select(x => new Models.OpeningTime
                    {
                        Day = x.WeekDay?.ToString(),
                        Time = x.Times,
                    }),
            };

            return pharmacyDetails;
        }
    }
}
