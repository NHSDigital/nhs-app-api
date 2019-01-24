using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using Name = NHSOnline.Backend.Worker.OrganDonation.Models.Name;
using Address = NHSOnline.Backend.Worker.OrganDonation.Models.Address;
using static NHSOnline.Backend.Worker.Support.ValidateAndLog.ValidationOptions;
using static NHSOnline.Backend.Worker.Constants.OrganDonationConstants;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal class RegistrationRequestMapper : IMapper<OrganDonationRegistrationRequest, RegistrationRequest>
    {
        private readonly IEnumMapper<string, FaithDeclaration> _faithDeclarationMapper;
        private readonly IEnumMapper<string, Decision> _decisionMapper;
        private readonly IMapper<string, Areas.OrganDonation.Models.Name, Name> _nameMapper;
        private readonly IMapper<string, Areas.OrganDonation.Models.Address, Address> _addressMapper;
        private readonly IOrganDonationDonationWishesMapper _donationWishesMapper;
        private readonly IOrganDonationGenderMapper _genderMapper;
        private readonly ILogger<RegistrationRequestMapper> _logger;

        public RegistrationRequestMapper(
            IEnumMapper<string, FaithDeclaration> faithDeclarationMapper,
            IEnumMapper<string, Decision> decisionMapper,
            IMapper<string, Areas.OrganDonation.Models.Name, Name> nameMapper,
            IMapper<string, Areas.OrganDonation.Models.Address, Address> addressMapper,
            IOrganDonationDonationWishesMapper donationWishesMapper,
            IOrganDonationGenderMapper genderMapper,
            ILogger<RegistrationRequestMapper> logger)
        {
            _faithDeclarationMapper = faithDeclarationMapper;
            _genderMapper = genderMapper;
            _decisionMapper = decisionMapper;
            _nameMapper = nameMapper;
            _addressMapper = addressMapper;
            _donationWishesMapper = donationWishesMapper;
            _logger = logger;
        }

        public RegistrationRequest Map(OrganDonationRegistrationRequest source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ThrowError)
                .IsNotNull(source?.Registration, nameof(source.Registration), ThrowError)
                .IsNotNull(source?.AdditionalDetails, nameof(source.AdditionalDetails), ThrowError)
                .IsValid();

            var registrationRequest = new RegistrationRequest
            {
                EthnicCategory = MapConcept(source.AdditionalDetails.EthnicityId),
                Address = MapList(_addressMapper.Map(source.Registration.AddressFull, source.Registration.Address)),
                BirthDate = source.Registration.DateOfBirth?.ToString(DateFormat, CultureInfo.InvariantCulture),
                Gender = _genderMapper.Map(source.Registration.Gender),
                OrganDonationDecision = _decisionMapper.From(source.Registration.Decision),
                ReligiousAffiliation = MapConcept(source.AdditionalDetails.ReligionId),
                Identifier = MapList(new Identifier { Value = source.Registration.NhsNumber }),
                Name = MapList(_nameMapper.Map(source.Registration.NameFull, source.Registration.Name))
            };

            if (source.Registration.Decision != Decision.OptIn)
                return registrationRequest;

            new ValidateAndLog(_logger)
                .IsNotNull(source.Registration.FaithDeclaration, nameof(source.Registration.FaithDeclaration),
                    ThrowError)
                .IsValid();

            registrationRequest.DonationWishes = _donationWishesMapper.Map(source.Registration.DecisionDetails);
            registrationRequest.FaithDeclaration =
                _faithDeclarationMapper.From(source.Registration.FaithDeclaration.Value);

            return registrationRequest;
        }

        private static List<T> MapList<T>(T entry) => new List<T> { entry };

        private CodeableConcept MapConcept(string code) =>
            new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding { Code = code }
                }
            };
    }
}