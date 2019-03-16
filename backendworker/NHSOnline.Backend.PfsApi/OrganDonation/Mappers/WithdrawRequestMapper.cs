using System.Globalization;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;
using Address = NHSOnline.Backend.PfsApi.OrganDonation.ApiModels.Address;
using Name = NHSOnline.Backend.PfsApi.OrganDonation.ApiModels.Name;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    internal class WithdrawRequestMapper : IMapper<OrganDonationWithdrawRequest, WithdrawRequest>
    {
        private readonly IMapper<Models.Name, Name> _nameMapper;
        private readonly IMapper<string, Models.Address, Address> _addressMapper;
        private readonly IOrganDonationIdentifierMapper _identifierMapper;
        private readonly IOrganDonationGenderMapper _genderMapper;

        public WithdrawRequestMapper(
            IMapper<Models.Name, Name> nameMapper,
            IMapper<string, Models.Address, Address> addressMapper,
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
