using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support
{
    public class OdsCodeMassager : IOdsCodeMassager
    {
        private const string OdsCodeRegex = @"([A-Z]\d{5}|[A-Z]\d[A-Z]\d[A-Z])";
        private static readonly string OdsCodeMapRegex = $"^{OdsCodeRegex}:{OdsCodeRegex}(;{OdsCodeRegex}:{OdsCodeRegex})*$";

        private static readonly IDictionary<string, string> DefaultOdsCodeMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "G85075", "X00100" },
            { "G85672", "X00100" }
        };

        private readonly IDictionary<string, string> _odsCodeMap;

        public bool IsEnabled { get; }

        private readonly ILogger<OdsCodeMassager> _logger;

        public OdsCodeMassager(IConfiguration configuration, ILogger<OdsCodeMassager> logger)
        {
            _logger = logger;

            IsEnabled = bool.TrueString.Equals(
                configuration.GetOrWarn("ODS_REMAP_ENABLED", _logger),
                StringComparison.OrdinalIgnoreCase);

            if (IsEnabled)
            {
                _odsCodeMap = BuildOdsCodeMap(configuration);
            }
        }

        private IDictionary<string, string> BuildOdsCodeMap(IConfiguration configuration)
        {
            var odsRemapMap = configuration.GetOrNull("ODS_REMAP_MAP");

            if (odsRemapMap == null)
            {
                _logger.LogWarning("Environment variable ODS_REMAP_MAP not set - using default ODS Code map");
                return DefaultOdsCodeMap;
            }

            if (!TryParseOdsRemapMap(odsRemapMap, out var odsCodeMap))
            {
                _logger.LogWarning("Unable to parse environment variable ODS_REMAP_MAP - using default ODS Code map");
                return DefaultOdsCodeMap;
            }

            return odsCodeMap;
        }

        private bool TryParseOdsRemapMap(string odsRemapMap, out Dictionary<string, string> odsCodeMap)
        {
            if (!Regex.IsMatch(odsRemapMap, OdsCodeMapRegex))
            {
                _logger.LogWarning("Unable to parse environment variable ODS_REMAP_MAP");
                odsCodeMap = new Dictionary<string, string>();
                return false;
            }

            odsCodeMap = odsRemapMap.Split(';')
                .Select(x => x.Split(':'))
                .ToDictionary(x => x[0], x => x[1], StringComparer.OrdinalIgnoreCase);

            return true;
        }

        public string CheckOdsCode(string odsCode)
        {
            if (!IsEnabled)
            {
                return odsCode;
            }

            if (odsCode != null && _odsCodeMap.TryGetValue(odsCode, out var mappedCode))
            {
                _logger.LogInformation($"Massaging ODS Code {odsCode} to {mappedCode}");

                return mappedCode;
            }

            return odsCode;
        }
    }
}
