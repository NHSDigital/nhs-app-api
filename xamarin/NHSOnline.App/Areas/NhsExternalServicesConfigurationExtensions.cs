using System;
using System.Net;
using NHSOnline.App.Config;

namespace NHSOnline.App.Areas
{
    internal static class NhsExternalServicesConfigurationExtensions
    {
        internal static Uri NhsUkContactUsUrlWithErrorCode(
            this INhsExternalServicesConfiguration configuration,
            string errorCode)
        {
            return new UriBuilder(configuration.NhsUkContactUsUrl)
            {
                Query = $"?errorCode={WebUtility.UrlEncode(errorCode)}"
            }.Uri;
        }
    }
}
