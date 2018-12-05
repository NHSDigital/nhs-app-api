using System;
using NHSOnline.Backend.Worker.Support.Hasher;

namespace NHSOnline.Backend.Worker.GpSystems.Linkage
{
    public class Im1CacheKeyGenerator : IIm1CacheKeyGenerator
    {
        private readonly IHashingService _hashingService;

        public Im1CacheKeyGenerator(IHashingService hashingService)
        {
            _hashingService = hashingService;
        }

        public string GenerateCacheKey(string accountId, string odsCode, string linkageKey)
        {
            if (string.IsNullOrEmpty(accountId) && 
                string.IsNullOrEmpty(odsCode) && 
                string.IsNullOrEmpty(linkageKey))
            {
                throw new ArgumentException("need to provide values to create key");
            }
            return _hashingService.Hash(odsCode + accountId + linkageKey);
        }
    }
}