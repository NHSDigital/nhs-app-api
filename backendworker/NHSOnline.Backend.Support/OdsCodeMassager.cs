using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support
{
    public class OdsCodeMassager : IOdsCodeMassager
    {
		private static readonly List<string> VisionCidOdsCodes = new List<string> { "G85075", "G85672" };

        private const string VisionTestOdsCode = "X00100";

        public bool IsEnabled { get; }

        private readonly ILogger<OdsCodeMassager> _logger;

        public OdsCodeMassager(IConfiguration configuration, ILogger<OdsCodeMassager> logger)
        {
            _logger = logger;

            IsEnabled = bool.TrueString.Equals(
                configuration.GetOrWarn("VISION_ODS_REMAP_ENABLED", _logger),
                StringComparison.OrdinalIgnoreCase);
        }

        public string CheckOdsCode(string odsCode)
        {
            if (IsEnabled && VisionCidOdsCodes.Contains(odsCode.ToUpper(CultureInfo.InvariantCulture)))
            {
                _logger.LogInformation($"Before we do the OdsCodeMassage, code is {odsCode}");
                return VisionTestOdsCode;
            }
            return odsCode;
        }
    }
}
