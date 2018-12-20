using System;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;
using Address = NHSOnline.Backend.Worker.Areas.OrganDonation.Models.Address;
using Name = NHSOnline.Backend.Worker.Areas.OrganDonation.Models.Name;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public class OrganDonationRegistrationMapper : IMapper<DemographicsResponse, OrganDonationRegistration>,
        IMapper<OrganDonationRegistration, OrganDonationSuccessResponse<RegistrationLookupResponse>,
            OrganDonationRegistration>
    {
        private const string AllChoiceKey = "all";
        private readonly IMapper<string, Decision> _organDonationDecisionMapper;
        private readonly IMapper<string, FaithDeclaration> _organDonationFaithDeclarationMapper;
        private readonly IMapper<string, ChoiceState> _organDonationChoiceStateMapper;

        public OrganDonationRegistrationMapper(IMapper<string, Decision> organDonationDecisionMapper,
            IMapper<string, FaithDeclaration> organDonationFaithDeclarationMapper,
            IMapper<string, ChoiceState> organDonationChoiceStateMapper)
        {
            _organDonationDecisionMapper = organDonationDecisionMapper;
            _organDonationChoiceStateMapper = organDonationChoiceStateMapper;
            _organDonationFaithDeclarationMapper = organDonationFaithDeclarationMapper;
        }

        public OrganDonationRegistration Map(DemographicsResponse source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new OrganDonationRegistration
            {
                Gender = source.Sex,
                AddressFull = source.Address,
                Address = MapAddress(source),
                NameFull = source.PatientName,
                Name = MapName(source),
                NhsNumber = source.NhsNumber,
                DateOfBirth = source.DateOfBirth,
            };
        }

        public OrganDonationRegistration Map(OrganDonationRegistration firstSource,
            OrganDonationSuccessResponse<RegistrationLookupResponse> secondSource)
        {
            if (firstSource == null)
                throw new ArgumentNullException(nameof(firstSource));

            if (secondSource?.Entry?.FirstOrDefault()?.Resource == null)
                throw new ArgumentNullException(nameof(secondSource));

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
                Decision = _organDonationDecisionMapper.Map(existingRegistration.OrganDonationDecision),
                Identifier = existingRegistration.Identifier.FirstOrDefault()?.Value,
                FaithDeclaration = _organDonationFaithDeclarationMapper.Map(existingRegistration.FaithDeclaration)
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

        private DecisionDetails MapDecisionDetails(RegistrationLookupResponse existingRegistration)
        {
            var decisionDetails = new DecisionDetails
            {
                All = _organDonationChoiceStateMapper.Map(existingRegistration.DonationWishes[AllChoiceKey]) ==
                      ChoiceState.Yes,
                Choices = existingRegistration.DonationWishes.Where(w =>
                        !string.Equals(w.Key, AllChoiceKey, StringComparison.Ordinal))
                    .Select(w => new Choice
                    {
                        Name = w.Key,
                        Value = _organDonationChoiceStateMapper.Map(w.Value)
                    })
                    .ToList()
            };

            return decisionDetails;
        }
    }
}