using System;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.Mappers
{
    public class ProofLevelMapper : IMapper<string, ProofLevel?>
    {
        private readonly ILogger _logger;

        public ProofLevelMapper(ILogger<ProofLevelMapper> logger)
        {
            _logger = logger;
        }

        public ProofLevel? Map(string source)
        {
            // NHSO-9061: Remove once supported by Login
            if (string.IsNullOrWhiteSpace(source))
            {
                return ProofLevel.P9;
            }

            if (Enum.TryParse(typeof(ProofLevel), source, true, out var proofLevel))
            {
                return (ProofLevel)proofLevel;
            }

            _logger.LogError($"Unsupported identity proofing level returned by Login: {source}");
            return null;
        }
    }
}
