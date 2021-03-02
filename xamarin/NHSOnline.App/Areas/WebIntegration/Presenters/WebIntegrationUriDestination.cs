using System;
using NHSOnline.App.Config;

namespace NHSOnline.App.Areas.WebIntegration.Presenters
{
    internal sealed class WebIntegrationUriDestination
    {
        private readonly INhsLoginConfiguration _nhsLoginConfiguration;
        private readonly Uri _integrationUri;

        internal WebIntegrationUriDestination(
            INhsLoginConfiguration nhsLoginConfiguration,
            Uri integrationUri)
        {
            _nhsLoginConfiguration = nhsLoginConfiguration;
            _integrationUri = integrationUri;
        }

        internal bool ShouldOpenInBrowserOverlay(Uri url)
        {
            if (IsSameHost(url))
            {
                return false;
            }

            if (IsNhsLoginHost(url))
            {
                return false;
            }

            return true;
        }

        private bool IsSameHost(Uri url) => string.Equals(url.Host, _integrationUri.Host, StringComparison.OrdinalIgnoreCase);

        private bool IsNhsLoginHost(Uri url) => url.Host.EndsWith(_nhsLoginConfiguration.BaseHost, StringComparison.OrdinalIgnoreCase);
    }
}