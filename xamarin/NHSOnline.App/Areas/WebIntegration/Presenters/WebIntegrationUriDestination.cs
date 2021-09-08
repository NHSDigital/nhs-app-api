using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.App.Config;

namespace NHSOnline.App.Areas.WebIntegration.Presenters
{
    internal sealed class WebIntegrationUriDestination
    {
        private readonly INhsLoginConfiguration _nhsLoginConfiguration;
        private readonly Uri _integrationUri;
        private readonly IReadOnlyCollection<Uri>? _additionalDomains;

        internal WebIntegrationUriDestination(
            INhsLoginConfiguration nhsLoginConfiguration,
            Uri integrationUri,
            IReadOnlyCollection<Uri>? additionalDomains)
        {
            _nhsLoginConfiguration = nhsLoginConfiguration;
            _integrationUri = integrationUri;
            _additionalDomains = additionalDomains;
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

            if (IsAdditionalHost(url))
            {
                return false;
            }

            return true;
        }

        private bool IsSameHost(Uri url)
            => string.Equals(url.Host, _integrationUri.Host, StringComparison.OrdinalIgnoreCase);

        private bool IsNhsLoginHost(Uri url)
            => url.Host.EndsWith(_nhsLoginConfiguration.BaseHost, StringComparison.OrdinalIgnoreCase);

        private bool IsAdditionalHost(Uri url) => _additionalDomains?.Any(additionalDomain =>
            url.Host.Equals(additionalDomain.Host, StringComparison.OrdinalIgnoreCase)) ?? false;
    }
}