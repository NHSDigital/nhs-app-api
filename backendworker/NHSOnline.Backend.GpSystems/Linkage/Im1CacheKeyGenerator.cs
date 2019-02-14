using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Hasher;

namespace NHSOnline.Backend.GpSystems.Linkage
{
    public class Im1CacheKeyGenerator : IIm1CacheKeyGenerator
    {
        private readonly IHashingService _hashingService;
        private readonly ILogger<Im1CacheKeyGenerator> _logger;

        public Im1CacheKeyGenerator(IHashingService hashingService, ILogger<Im1CacheKeyGenerator> logger)
        {
            _hashingService = hashingService;
            _logger = logger;
        }

        public string GenerateCacheKey(string accountId, string odsCode, string linkageKey)
        {
            if (string.IsNullOrEmpty(accountId) &&
                string.IsNullOrEmpty(odsCode) &&
                string.IsNullOrEmpty(linkageKey))
            {
                _logger.LogError("no values provided to key generator");
                throw new ArgumentException("need to provide values to create key");
            }

            return _hashingService.Hash(odsCode + accountId + linkageKey);
        }
    }
}