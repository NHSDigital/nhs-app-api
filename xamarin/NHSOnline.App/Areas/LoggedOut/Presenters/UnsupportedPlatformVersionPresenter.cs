using System.Threading.Tasks;
using NHSOnline.App.Areas.LoggedOut.Models;
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
        private readonly UnsupportedPlatformVersionModel _unsupportedPlatformVersionModel;

        public UnsupportedPlatformVersionPresenter(
            IUnsupportedPlatformVersionView view,
            IBrowserOverlay browserOverlay,
            INhsExternalServicesConfiguration externalServicesConfiguration,
            UnsupportedPlatformVersionModel unsupportedPlatformVersionModel)
        {
            _view = view;
            _browserOverlay = browserOverlay;
            _externalServicesConfiguration = externalServicesConfiguration;
            _unsupportedPlatformVersionModel = unsupportedPlatformVersionModel;

            _view.MinimumPlatformVersion = _unsupportedPlatformVersionModel.MinimumPlatformVersion;

            _view.AppNavigation
                .RegisterHandler(ViewCovidPassRequested, (view, handler) => view.CovidPassRequested = handler)
                .RegisterHandler(NhsAppOnlineLoginRequested, (view, handler) => view.NhsAppOnlineLoginRequested = handler)
                .RegisterHandler(ViewOnOneOneOneRequested, (view, handler) => view.OneOneOneRequested = handler);
        }

        private async Task ViewCovidPassRequested()
        {
            await _browserOverlay
                .OpenBrowserOverlay(_externalServicesConfiguration.CovidPassUrl)
                .PreserveThreadContext();
        }

        private async Task NhsAppOnlineLoginRequested()
        {
            await _browserOverlay
                .OpenBrowserOverlay(_externalServicesConfiguration.NhsAppOnlineLogin)
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