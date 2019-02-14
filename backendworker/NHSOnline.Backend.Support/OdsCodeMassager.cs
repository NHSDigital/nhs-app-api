using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support
{
    public static class OdsCodeMassager
    {
		private readonly static List<string> VisionCIDOdsCodes = new List<string> { "G85075", "G85672" };
        private const string VisionTestODSCode = "X00100";
        public static bool IsEnabled { get; set; } = false;

        public static string CheckOdsCode(string odsCode, ILogger logger)
        {
            if (IsEnabled && VisionCIDOdsCodes.Contains(odsCode.ToUpper(CultureInfo.InvariantCulture)))
            {
                logger.LogInformation($"Before we do the OdsCodeMassage, code is {odsCode}");
                return VisionTestODSCode;
            }
            return odsCode;
        }
    }
}
