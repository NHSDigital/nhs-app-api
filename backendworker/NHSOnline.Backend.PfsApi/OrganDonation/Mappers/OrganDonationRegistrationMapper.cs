using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;
using static NHSOnline.Backend.Support.Constants.OrganDonationConstants;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;
using Address = NHSOnline.Backend.PfsApi.OrganDonation.Models.Address;
using Name = NHSOnline.Backend.PfsApi.OrganDonation.Models.Name;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    internal class OrganDonationRegistrationMapper :
        IMapper<DemographicsResponse, P9UserSession, OrganDonationRegistration>,
        IMapper<OrganDonationRegistration, RegistrationLookupResponse, OrganDonationRegistration>
    {
        private readonly IEnumMapper<string, Decision> _organDonationDecisionMapper;
        private readonly IEnumMapper<string, FaithDeclaration> _organDonationFaithDeclarationMapper;
        private readonly IEnumMapper<string, ChoiceState> _organDonationChoiceStateMapper;
        private readonly ILogger<OrganDonationRegistrationMapper> _logger;

        public OrganDonationRegistrationMapper(IEnumMapper<string, Decision> organDonationDecisionMapper,
            IEnumMapper<string, FaithDeclaration> organDonationFaithDeclarationMapper,
            IEnumMapper<string, ChoiceState> organDonationChoiceStateMapper,
            ILogger<OrganDonationRegistrationMapper> logger)
        {
            _organDonationDecisionMapper = organDonationDecisionMapper;
            _organDonationChoiceStateMapper = organDonationChoiceStateMapper;
            _organDonationFaithDeclarationMapper = organDonationFaithDeclarationMapper;
            _logger = logger;
        }

        public OrganDonationRegistration Map(DemographicsResponse firstSource, P9UserSession secondSource)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(firstSource, nameof(firstSource), ThrowError)
                .IsNotNull(secondSource, nameof(secondSource), ThrowError)
                .IsValid();

            return new OrganDonationRegistration
            {
                Gender = firstSource.Sex,
                AddressFull = firstSource.Address,
                Address = MapAddress(firstSource),
                NameFull = secondSource.CitizenIdUserSession.Name,
                Name = MapName(secondSource.CitizenIdUserSession),
                NhsNumber = secondSource.NhsNumber,
                DateOfBirth = secondSource.CitizenIdUserSession.DateOfBirth
            };
        }

        public OrganDonationRegistration Map(
            OrganDonationRegistration firstSource,
            RegistrationLookupResponse secondSource)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(firstSource, nameof(firstSource), ThrowError)
                .IsNotNull(secondSource, nameof(secondSource), ThrowError)
                .IsNotNull(secondSource?.Entry, nameof(secondSource.Entry), ThrowError)
                .IsValid();

            var existingRegistration = secondSource.Entry.FirstOrDefault()?.Resource;

            new ValidateAndLog(_logger)
                .IsNotNull(existingRegistration, nameof(existingRegistration), ThrowError)
                .IsValid();

            var result = new OrganDonationRegistration
            {
                AddressFull = firstSource.AddressFull,
                Address = MapAddress(firstSource),
                NameFull = firstSource.NameFull,
                Name = MapName(firstSource),
                Gender = firstSource.Gender,
                NhsNumber = firstSource.NhsNumber,
                DateOfBirth = firstSource.DateOfBirth,
                Decision = _organDonationDecisionMapper.To(existingRegistration.OrganDonationDecision),
                Identifier = existingRegistration.Id,
                FaithDeclaration = _organDonationFaithDeclarationMapper.To(existingRegistration.FaithDeclaration),
                State = State.Ok
            };

            if (result.Decision == Decision.OptIn)
            {
                result.DecisionDetails = MapDecisionDetails(existingRegistration);
            }

            return result;
        }

        private static Address MapAddress(OrganDonationRegistration organDonationRegistration)
        {
            var address = organDonationRegistration.Address != null
                ? new Address
                {
                    HouseName = organDonationRegistration.Address.HouseName,
                    NumberStreet = organDonationRegistration.Address.NumberStreet,
                    Village = organDonationRegistration.Address.Village,
                    Town = organDonationRegistration.Address.Town,
                    County = organDonationRegistration.Address.County,
                    PostCode = organDonationRegistration.Address.PostCode
                }
                : null;

            return address;
        }

        private static Address MapAddress(DemographicsResponse demographicsResponse)
        {
            var address = demographicsResponse.AddressParts != null
                ? new Address
                {
                    HouseName = demographicsResponse.AddressParts.HouseName,
                    NumberStreet = demographicsResponse.AddressParts.NumberStreet,
                    Village = demographicsResponse.AddressParts.Village,
                    Town = demographicsResponse.AddressParts.Town,
                    County = demographicsResponse.AddressParts.County,
                    PostCode = demographicsResponse.AddressParts.Postcode
                }
                : null;

            return address;
        }

        private static Name MapName(OrganDonationRegistration organDonationRegistration)
        {
            var name = organDonationRegistration.Name != null
                ? new Name
                {
                    Title = organDonationRegistration.Name.Title,
                    GivenName = organDonationRegistration.Name.GivenName,
                    Surname = organDonationRegistration.Name.Surname
                }
                : null;

            return name;
        }

        private static Name MapName(CitizenIdUserSession citizenIdUserSession)
        {
            return new Name
            {
                Surname = citizenIdUserSession.FamilyName,
                GivenName = citizenIdUserSession.GivenName
            };
        }

        private DecisionDetails MapDecisionDetails(Registration existingRegistration)
        {
            var overallDecision =
                _organDonationChoiceStateMapper.To(existingRegistration.DonationWishes[AllOrgansChoiceKey]);

            var choiceBreakdown = existingRegistration.DonationWishes.Where(choice =>
                !string.Equals(choice.Key, AllOrgansChoiceKey, StringComparison.Ordinal));

            var decisionDetails = new DecisionDetails
            {
                All = overallDecision == ChoiceState.Yes,
                Choices = choiceBreakdown.ToDictionary(x => x.Key, x => _organDonationChoiceStateMapper.To(x.Value))
            };

            return decisionDetails;
        }
    }
}
