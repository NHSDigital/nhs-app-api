using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hl7.Fhir.Utility;
using NHSOnline.HttpMocks.Nhs.Models;
using NHSOnline.HttpMocks.Spine;

namespace NHSOnline.HttpMocks.Nhs
{
    [Route("nhs-search-indexes/service-search")]
    public class GpSearchController : Controller
    {
        private const string DefaultNominatedPharmacyOrgName = "Walter White Chemists";
        private const string DefaultNominatedPharmacySearchFilter = "ODSCode eq '" + SpineResponseBuilder.NominatedPharmacyOdsCode + "'";
        private const string RandomInternetPharmacySearchFilter =
            "(OrganisationTypeId eq 'PHA') and (OrganisationSubType eq 'DistanceSelling')";

        [HttpPost("search")]
        public IActionResult OrganisationSearch([FromBody] OrganisationSearchData organisationSearchData)
        {
            List<OrganisationItem> organisations = new List<OrganisationItem>();

            if (organisationSearchData?.Filter!.Equals
                    (RandomInternetPharmacySearchFilter, StringComparison.OrdinalIgnoreCase) == true)
            {
                if (string.IsNullOrEmpty(organisationSearchData.Search))
                {
                    organisations = GetRandomOnlineOrganisations();
                }
                else
                {
                    organisations = GetOnlineOrganisationsByName(organisationSearchData.Search);
                }
            }
            else if (organisationSearchData?.Filter!.Equals
                         (DefaultNominatedPharmacySearchFilter, StringComparison.OrdinalIgnoreCase) == true)
            {
                organisations.Add(GetHighStreetPharmacy(DefaultNominatedPharmacyOrgName, 0));
            }
            else
            {
                for (int i = 0; i < organisationSearchData?.Top; i++)
                {
                    var orgName = i == 0 ? DefaultNominatedPharmacyOrgName : "Pharmacy " + i;
                    organisations.Add(GetHighStreetPharmacy(orgName, i));
                }
            }

            return Ok(new Organisation
            {
                Value = organisations,
                Count = organisations.Count
            });
        }

        [HttpPost("postcodesandplaces/search")]
        public PostCodeAndPlaces PostCodeSearch()
        {
            return new PostCodeAndPlaces
            {
                value = new List<PostCodeAndPlacesItem>
                {
                    new PostCodeAndPlacesItem
                    {
                        Latitude = "53.8008",
                        Longitude = "1.5491"
                    }
                },
                count = 1
            };
        }

        private static List<OrganisationItem> GetRandomOnlineOrganisations()
        {
            List<OrganisationItem> organisations = new List<OrganisationItem>();

            for (int i = 0; i < 450; i++)
            {
                organisations.Add(new OrganisationItem
                {
                    OrganisationName = $"Internet Pharmacy {i}",
                    Url = $"www.internet_pharmacy_{i}.com",
                    ODSCode = SpineResponseBuilder.NominatedPharmacyOdsCode,
                    Contacts = GetContacts(i)
                });
            }
            return organisations;
        }

        private static List<OrganisationItem> GetOnlineOrganisationsByName(string searchTerm)
        {
            List<OrganisationItem> organisations = new List<OrganisationItem>();

            for (int i = 0; i < 20; i++)
            {
                organisations.Add(new OrganisationItem
                {
                    OrganisationName = $"Internet Pharmacy {searchTerm} {i}",
                    Url = $"www.internet_pharmacy_{searchTerm}_{i}.com",
                    ODSCode = SpineResponseBuilder.NominatedPharmacyOdsCode,
                    Contacts = GetContacts(i)
                });
            }
            return organisations;
        }

        private static OrganisationItem GetHighStreetPharmacy(string orgName, int index)
        {
            return new OrganisationItem
            {
                OrganisationName = orgName,
                Address1 = index + 1 + " Address Street",
                Address2 = "",
                Address3 = "",
                City = "Leeds",
                County = "Yorkshire",
                Postcode = "LS1 1AB",
                ODSCode = SpineResponseBuilder.NominatedPharmacyOdsCode,
                Metrics = new List<Metric>
                {
                    new Metric { MetricName = "Electronic prescription service", Value = "Yes" }
                },
                OrganisationSubType = "Community",
                Geocode = new Geocode
                {
                    Coordinates = new Collection<double> { 53.8008, 1.5491 },
                },
            };
        }

        private static List<Contact> GetContacts(int index)
        {
            return new List<Contact>
            {
                new Contact
                {
                    ContactType = "Primary",
                    ContactAvailabilityType = "24/7",
                    ContactMethodType = "Email",
                    ContactValue = $"email{index}@gmail.com"
                },
                new Contact
                {
                    ContactType = "Primary",
                    ContactAvailabilityType = "24/7",
                    ContactMethodType = "Website",
                    ContactValue = $"www.website{index}.com"
                },
                new Contact
                {
                    ContactType = "Primary",
                    ContactAvailabilityType = "Office hours",
                    ContactMethodType = "Telephone",
                    ContactValue = "0191 3402425"
                },
                new Contact
                {
                    ContactType = "Primary",
                    ContactAvailabilityType = "Office hours",
                    ContactMethodType = "Fax",
                    ContactValue = "0191 3402425"
                },
            };
        }
    }
}