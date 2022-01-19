using System.Threading.Tasks;
using NHSOnline.App.Config;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class UnsupportedPlatformVersionPresenter
    {
        private readonly IUnsupportedPlatformVersionView _view;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public UnsupportedPlatformVersionPresenter(
            IUnsupportedPlatformVersionView view,
            IBrowserOverlay browserOverlay,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _browserOverlay = browserOverlay;
            _externalServicesConfiguration = externalServicesConfiguration;

            _view.AppNavigation
                .RegisterHandler(NhsAppOnlineLoginRequested, (view, handler) => view.NhsAppOnlineLoginRequested = handler)
                .RegisterHandler(NhsAppTechnicalIssuesSupportRequested, (view, handler) => view.NhsAppTechnicalIssuesSupportRequested = handler)
                .RegisterHandler(ViewOnOneOneOneRequested, (view, handler) => view.OneOneOneRequested = handler);
        }

        private async Task NhsAppOnlineLoginRequested()
        {
            await _browserOverlay
                .OpenBrowserOverlay(_externalServicesConfiguration.NhsAppOnlineLogin)
                .PreserveThreadContext();
        }

        private async Task NhsAppTechnicalIssuesSupportRequested()
        {
            await _browserOverlay
                .OpenBrowserOverlay(_externalServicesConfiguration.NhsAppTechnicalIssuesSupportUrl)
                .PreserveThreadContext();
        }

        private async Task ViewOnOneOneOneRequested()
        {
            await _browserOverlay
                .OpenBrowserOverlay(_externalServicesConfiguration.OneOneOneUrl)
                .PreserveThreadContext();
        }
    }
}