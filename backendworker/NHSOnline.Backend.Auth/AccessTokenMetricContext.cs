using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auth
{
    public sealed class AccessTokenMetricContext : IMetricContext
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ILogger _logger;
        private readonly IMapper<string, ProofLevel?> _proofLevelMapper;

        public AccessTokenMetricContext(IUserProfileService userProfileService,
            ILogger<AccessTokenMetricContext> logger, IMapper<string, ProofLevel?> proofLevelMapper)
        {
            _userProfileService = userProfileService;
            _logger = logger;
            _proofLevelMapper = proofLevelMapper;
        }
        public string NhsLoginId => AccessToken.Parse(_logger, UserProfile.AccessToken).Subject;
        public ProofLevel ProofLevel => GetProofLevel();
        public string OdsCode => UserProfile.OdsCode;

        private UserProfile UserProfile => _userProfileService.GetExistingUserProfileOrThrow();

        private ProofLevel GetProofLevel()
        {
            var proofLevel = _proofLevelMapper.Map(UserProfile.IdentityProofingLevel);
            if (!proofLevel.HasValue)
            {
                throw new InvalidOperationException("Proof level cannot be determined");
            }

            return proofLevel.Value;
        }
    }
}