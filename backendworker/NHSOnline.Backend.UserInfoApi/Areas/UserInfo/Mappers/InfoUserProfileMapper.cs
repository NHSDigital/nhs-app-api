using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Models;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Mappers
{
    internal class InfoUserProfileMapper : IMapper<UserProfile, InfoUserProfile>
    {
        private readonly IMapper<string, ProofLevel?> _proofLevelMapper;
        private readonly ILogger _logger;

        public InfoUserProfileMapper(IMapper<string, ProofLevel?> proofLevelMapper, ILogger<InfoUserProfileMapper> logger)
        {
            _proofLevelMapper = proofLevelMapper;
            _logger = logger;
        }

        public InfoUserProfile Map(UserProfile source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ThrowError)
                .IsValid();

            var proofLevel = _proofLevelMapper.Map(source.IdentityProofingLevel);

            return new InfoUserProfile
            {
                NhsNumber = proofLevel == ProofLevel.P9 ? source.NhsNumber : null,
                OdsCode = source.OdsCode,
                Email = source.Email
            };
        }
    }
}
