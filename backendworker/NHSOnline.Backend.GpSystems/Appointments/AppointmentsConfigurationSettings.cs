using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Appointments
{
    public class AppointmentsConfigurationSettings
    {
        public bool SixteenWeeksSlotsEnabled { get; set; }

        public AppointmentsConfigurationSettings(bool sixteenWeeksSlotsEnabled)
        {
            SixteenWeeksSlotsEnabled = sixteenWeeksSlotsEnabled;
        }

        public static AppointmentsConfigurationSettings CreateAndValidate(IConfiguration configuration, ILogger logger)
        {
            var sixteenWeeksSlotsEnabled = bool.TrueString.Equals(configuration.GetOrThrow("SIXTEEN_WEEKS_SLOTS_ENABLED", logger), StringComparison.OrdinalIgnoreCase);
            return new AppointmentsConfigurationSettings(sixteenWeeksSlotsEnabled);
        }
    }
}