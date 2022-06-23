using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Config;
using NHSOnline.App.Controls.WebViews;

namespace NHSOnline.App.Areas.WebIntegration.Presenters
{
    internal class SingleSignOnMonitor
    {
        private readonly INhsLoginConfiguration _nhsLoginConfiguration;
        private readonly ILogger<WebIntegrationPresenter> _logger;

        public SingleSignOnMonitor(
            INhsLoginConfiguration nhsLoginConfiguration,
            ILogger<WebIntegrationPresenter> logger)
        {
            _nhsLoginConfiguration = nhsLoginConfiguration;
            _logger = logger;
        }

        public void PageLoadComplete(WebViewPageLoadEventArgs pageLoadEventArgs)
        {
            var (finalUri, _) = pageLoadEventArgs.Urls.LastOrDefault();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse - finalUri can be null
            if (finalUri != null && IsNhsLoginEnterEmailScreen(finalUri))
            {
                var urls = string.Join("\n", pageLoadEventArgs.Urls.Select(x => x.Item1.GetLeftPart(UriPartial.Path)));
                _logger.LogError(
                    "Web integration page load ended up on NHS login enter-email screen. Redirect flow was:\n{Urls}",
                    urls);
            }
        }

        private bool IsNhsLoginEnterEmailScreen(Uri uri)
        {
            return IsNhsLoginHost() && IsEnterEmailPath();

            bool IsNhsLoginHost()
                => uri.Host.EndsWith(_nhsLoginConfiguration.BaseHost, StringComparison.OrdinalIgnoreCase);

            bool IsEnterEmailPath() => uri.AbsolutePath.Equals("/enter-email", StringComparison.OrdinalIgnoreCase);
        }
    }
}