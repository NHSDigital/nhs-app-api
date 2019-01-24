using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;
using static NHSOnline.Backend.Worker.Constants.OrganDonationConstants;
using static NHSOnline.Backend.Worker.Support.ValidateAndLog.ValidationOptions;
using Address = NHSOnline.Backend.Worker.Areas.OrganDonation.Models.Address;
using Name = NHSOnline.Backend.Worker.Areas.OrganDonation.Models.Name;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal class OrganDonationRegistrationMapper : IMapper<DemographicsResponse, OrganDonationRegistration>,
        IMapper<OrganDonationRegistration, RegistrationLookupResponse,
            OrganDonationRegistration>
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

        public OrganDonationRegistration Map(DemographicsResponse source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ThrowError)
                .IsValid();

            return new OrganDonationRegistration
            {
                Gender = source.Sex,
                AddressFull = source.Address,
                Address = MapAddress(source),
                NameFull = source.PatientName,
                Name = MapName(source),
                NhsNumber = source.NhsNumber,
                DateOfBirth = source.DateOfBirth
            };
        }

        public OrganDonationRegistration Map(OrganDonationRegistration firstSource,
            RegistrationLookupResponse secondSource)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(firstSource, nameof(firstSource), ThrowError)
                .IsNotNull(secondSource, nameof(secondSource), ThrowError)
                .IsValid();

            var existingRegistration = secondSource.Entry.First().Resource;

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
                Identifier = existingRegistration.Identifier.FirstOrDefault()?.Value,
                FaithDeclaration = _organDonationFaithDeclarationMapper.To(existingRegistration.FaithDeclaration)
            };

            if (result.Decision == Decision.OptIn)
                result.DecisionDetails = MapDecisionDetails(existingRegistration);

            return result;
        }

        private static Address MapAddress(OrganDonationRegistration organDonationRegistration)
        {
            var address = organDonationRegistration.Address != null
                ? new Address
                {
                    Text = organDonationRegistration.Address.Text,
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
                    Text = demographicsResponse.AddressParts.Text,
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

        private static Name MapName(DemographicsResponse demographicsResponse)
        {
            var name = demographicsResponse.NameParts != null
                ? new Name
                {
                    Title = demographicsResponse.NameParts.Title,
                    GivenName = demographicsResponse.NameParts.Given,
                    Surname = demographicsResponse.NameParts.Surname
                }
                : null;

            return name;
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