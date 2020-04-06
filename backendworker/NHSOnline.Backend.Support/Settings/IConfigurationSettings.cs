using System;

namespace NHSOnline.Backend.Support.Settings
{
    public interface IConfigurationSettings
    {
        string CookieDomain { get; set; }

        int? PrescriptionsDefaultLastNumberMonthsToDisplay { get; set; }

        int DefaultSessionExpiryMinutes { get; set; }

        int DefaultHttpTimeoutSeconds { get; set; }

        int MinimumAppAge { get; set; }

        int MinimumLinkageAge { get; set; }
    }
}