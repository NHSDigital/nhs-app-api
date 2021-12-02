using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices;
using NHSOnline.App.Droid.Renderers.WebViews;
using NHSOnline.App.Logging;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidPreHomeLogoutMonitor))]
namespace NHSOnline.App.Droid.DependencyServices
{
    internal class AndroidPreHomeLogoutMonitor: IPreHomeLogoutMonitor
    {
        private readonly ILogger _logger = NhsAppLogging.CreateLogger<AndroidPreHomeLogoutMonitor>();

        private DateTimeOffset _monitoringStartTime;
        private List<(Uri, DateTimeOffset)> _urls = new();

        public void Begin()
        {
            _urls = new List<(Uri, DateTimeOffset)>();
            _monitoringStartTime = DateTimeOffset.UtcNow;
        }

        public void PageLoadComplete(WebViewPageNavigationEventArgs pageNavigationEventArgs)
            => _urls.AddRange(pageNavigationEventArgs.Urls);

        public void Finish(Uri? finalUrl)
        {
            if (!LoggedOutWithinTwoMinutes())
            {
                return;
            }

            var urlLogs = BuildUrlLogs(finalUrl);

            _logger.LogError($"Pre-home journey logged out unexpectedly. Full UserAgent: {BaseWebViewRenderer.UserAgent}. Navigated to urls: {urlLogs}");
        }

        private bool LoggedOutWithinTwoMinutes()
            => _monitoringStartTime.AddMinutes(2).ToUnixTimeMilliseconds() >= DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        private string BuildUrlLogs(Uri? finalUrl)
        {
            var finalUrlPart = finalUrl?.GetLeftPart(UriPartial.Path);
            var navigatedToUrls = _urls.Count != 0;

            var urls = navigatedToUrls
                ? string.Join("\n", _urls.Select(x => x.Item1.GetLeftPart(UriPartial.Path)))
                : string.Empty;

            if (finalUrlPart != null && !LastUrlMatchesFinalUrl(finalUrlPart))
            {
                urls += "\n" + finalUrlPart;
            }

            return urls;
        }

        private bool LastUrlMatchesFinalUrl(string finalUrl)
            => _urls.Count != 0 && string.Equals(finalUrl, _urls.Last().Item1.GetLeftPart(UriPartial.Path), StringComparison.Ordinal);
    }
}