using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using GeoCoordinatePortable;
using static NHSOnline.Backend.Worker.GpSearch.ResponseEnums;
using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy
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
                OdsCode = pharmacy.NACSCode,
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


        public IEnumerable<PharmacyDetailsResponse> Map(IEnumerable<Organisation> pharmacies, GeoCoordinate postcodeCoordinate)
        {
            foreach (var pharmacy in pharmacies)
            {
                yield return Map(pharmacy, postcodeCoordinate);
            }
        }

        private PharmacyDetailsResponse Map(Organisation pharmacy, GeoCoordinate postcodeCoordinate)
        {
            var result = Map(pharmacy);

            if (postcodeCoordinate != null)
            {
                try
                {
                    result.Distance = GetDistanceInMiles(
                        postcodeCoordinate.GetDistanceTo(new GeoCoordinate(
                            pharmacy.Geocode.Coordinates[1],
                            pharmacy.Geocode.Coordinates[0])));
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    _logger.LogError(ex, "Invalid GeoCoordinates");
                }
            }          
            return result;
        }

        private static double GetDistanceInMiles(double distanceInMeters)
        {
            return Math.Round((distanceInMeters / 1609.344), 1);
        }
    }
}
