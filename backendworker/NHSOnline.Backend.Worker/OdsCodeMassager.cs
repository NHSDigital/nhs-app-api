using System;
using System.Collections.Generic;
using System.Globalization;

namespace NHSOnline.Backend.Worker
{
    public static class OdsCodeMassager
    {
		private readonly static List<string> VisionCIDOdsCodes = new List<string> { "G85075", "G85672" };
        private const string VisionTestODSCode = "X00100";
        public static bool IsEnabled { get; set; } = false;

        public static string CheckOdsCode(string odsCode)
        {
            if (IsEnabled && VisionCIDOdsCodes.Contains(odsCode.ToUpper(CultureInfo.InvariantCulture)))
            {
                return VisionTestODSCode;
            }
            return odsCode;
        }
    }
}
