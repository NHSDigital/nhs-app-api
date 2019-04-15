using System.Globalization;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using Name = NHSOnline.Backend.PfsApi.OrganDonation.ApiModels.Name;
using Address = NHSOnline.Backend.PfsApi.OrganDonation.ApiModels.Address;
using static NHSOnline.Backend.Support.Constants.OrganDonationConstants;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    internal class RegistrationRequestMapper : IMapper<OrganDonationRegistrationRequest, RegistrationRequest>
    {
        private readonly IEnumMapper<string, FaithDeclaration> _faithDeclarationMapper;
        private readonly IEnumMapper<string, Decision> _decisionMapper;
        private readonly IMapper<Models.Name, Name> _nameMapper;
        private readonly IMapper<string, Models.Address, Address> _addressMapper;
        private readonly IOrganDonationIdentifierMapper _identifierMapper;
        private readonly IOrganDonationDonationWishesMapper _donationWishesMapper;
        private readonly IOrganDonationGenderMapper _genderMapper;
        private readonly ILogger<RegistrationRequestMapper> _logger;

        public RegistrationRequestMapper(
            IEnumMapper<string, FaithDeclaration> faithDeclarationMapper,
            IEnumMapper<string, Decision> decisionMapper,
            IMapper<OrganDonation.Models.Name, Name> nameMapper,
            IMapper<string, OrganDonation.Models.Address, Address> addressMapper,
            IOrganDonationIdentifierMapper identifierMapper,
            IOrganDonationDonationWishesMapper donationWishesMapper,
            IOrganDonationGenderMapper genderMapper,
            ILogger<RegistrationRequestMapper> logger)
        {
            _faithDeclarationMapper = faithDeclarationMapper;
            _genderMapper = genderMapper;
            _decisionMapper = decisionMapper;
            _nameMapper = nameMapper;
            _addressMapper = addressMapper;
            _identifierMapper = identifierMapper;
            _donationWishesMapper = donationWishesMapper;
            _logger = logger;
        }

        public RegistrationRequest Map(OrganDonationRegistrationRequest source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ThrowError)
                .IsValid();

            var registrationRequest = new RegistrationRequest
            {
                Id = source.Registration.Identifier,
                EthnicCategory = string.IsNullOrWhiteSpace(source.AdditionalDetails.EthnicityId) 
                    ? null 
                    : MapperHelpers.MapConcept(EthnicityCodingSystem ,source.AdditionalDetails.EthnicityId),
                Address = MapperHelpers.MapList(_addressMapper.Map(source.Registration.AddressFull, source.Registration.Address)),
                BirthDate = source.Registration.DateOfBirth?.ToString(DateFormat, CultureInfo.InvariantCulture),
                Gender = _genderMapper.Map(source.Registration.Gender),
                OrganDonationDecision = _decisionMapper.From(source.Registration.Decision),
                ReligiousAffiliation = string.IsNullOrWhiteSpace(source.AdditionalDetails.ReligionId) 
                    ? null 
                    : MapperHelpers.MapConcept(ReligiousCodingSystem ,source.AdditionalDetails.ReligionId),
                Identifier = MapperHelpers.MapList(_identifierMapper.Map(source.Registration.NhsNumber)),
                Name = MapperHelpers.MapList(_nameMapper.Map(source.Registration.Name))
            };

            if (source.Registration.Decision != Decision.OptIn)
                return registrationRequest;

            registrationRequest.DonationWishes = _donationWishesMapper.Map(source.Registration.DecisionDetails);
            registrationRequest.FaithDeclaration = _faithDeclarationMapper.From(source.Registration.FaithDeclaration);

            return registrationRequest;
        }
    }
}