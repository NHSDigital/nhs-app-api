using System.Globalization;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;
using Address = NHSOnline.Backend.PfsApi.OrganDonation.Models.Address;
using Name = NHSOnline.Backend.PfsApi.OrganDonation.Models.Name;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    internal class WithdrawRequestMapper : IMapper<OrganDonationWithdrawRequest, WithdrawRequest>
    {
        private readonly IMapper<Name, ApiModels.Name> _nameMapper;
        private readonly IMapper<string, Address, ApiModels.Address> _addressMapper;
        private readonly IOrganDonationIdentifierMapper _identifierMapper;
        private readonly IOrganDonationGenderMapper _genderMapper;

        public WithdrawRequestMapper(
            IMapper<Name, ApiModels.Name> nameMapper,
            IMapper<string, Address, ApiModels.Address> addressMapper,
            IOrganDonationIdentifierMapper identifierMapper,
            IOrganDonationGenderMapper genderMapper)
        {
            _genderMapper = genderMapper;
            _nameMapper = nameMapper;
            _addressMapper = addressMapper;
            _identifierMapper = identifierMapper;
        }

        public WithdrawRequest Map(OrganDonationWithdrawRequest source)
        {
            var registrationRequest = new WithdrawRequest
            {
                Id = source.Identifier,
                Address = MapperHelpers.MapList(_addressMapper.Map(source.AddressFull, source.Address)),
                BirthDate = source.DateOfBirth?.ToString(Constants.OrganDonationConstants.DateFormat, CultureInfo.InvariantCulture),
                Gender = _genderMapper.Map(source.Gender),
                Identifier = MapperHelpers.MapList(_identifierMapper.Map( source.NhsNumber)),
                Name = MapperHelpers.MapList(_nameMapper.Map(source.Name)),
                WithdrawReason = MapperHelpers.MapConcept(Constants.OrganDonationConstants.WithdrawReasonCodingSystem, source.WithdrawReasonId)
            };

            return registrationRequest;
        }
    }
}
