using System.Threading.Tasks;
using NHSOnline.App.Config;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class UnsupportedPlatformVersionPresenter
    {
        private readonly IUnsupportedPlatformVersionView _view;
        private readonly IBrowser _browser;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public UnsupportedPlatformVersionPresenter(
            IUnsupportedPlatformVersionView view,
            IBrowser browser,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _browser = browser;
            _externalServicesConfiguration = externalServicesConfiguration;

            _view.AppNavigation
                .RegisterHandler(NhsAppOnlineLoginRequested, (view, handler) => view.NhsAppOnlineLoginRequested = handler)
                .RegisterHandler(NhsAppTechnicalIssuesSupportRequested, (view, handler) => view.NhsAppTechnicalIssuesSupportRequested = handler)
                .RegisterHandler(ViewOnOneOneOneRequested, (view, handler) => view.OneOneOneRequested = handler);
        }

        private async Task NhsAppOnlineLoginRequested()
        {
            await _browser
                .OpenBrowserOverlay(_externalServicesConfiguration.NhsAppOnlineLogin)
                .PreserveThreadContext();
        }

        private async Task NhsAppTechnicalIssuesSupportRequested()
        {
            await _browser
                .OpenBrowserOverlay(_externalServicesConfiguration.NhsAppTechnicalIssuesSupportUrl)
                .PreserveThreadContext();
        }

        private async Task ViewOnOneOneOneRequested()
        {
            await _browser
                .OpenBrowserOverlay(_externalServicesConfiguration.OneOneOneUrl)
                .PreserveThreadContext();
        }
    }
}