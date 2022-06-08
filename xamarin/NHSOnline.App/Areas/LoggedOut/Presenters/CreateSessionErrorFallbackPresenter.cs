using System.Threading.Tasks;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class CreateSessionErrorFallbackPresenter
    {
        private readonly ICreateSessionErrorFallbackView _view;
        private readonly CreateSessionErrorFallbackModel _model;
        private readonly IBrowser _browser;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public CreateSessionErrorFallbackPresenter(
            ICreateSessionErrorFallbackView view,
            CreateSessionErrorFallbackModel model,
            IBrowser browser,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _model = model;
            _browser = browser;
            _externalServicesConfiguration = externalServicesConfiguration;

            _view.AppNavigation
                .RegisterHandler(ViewOnOneOneOneRequested, (view, handler) => view.OneOneOneRequested = handler)
                .RegisterHandler(ViewOnBackHomeRequested, (view, handler) => view.BackHomeRequested = handler);
        }

        private async Task ViewOnOneOneOneRequested()
        {
            await _browser
                .OpenBrowserOverlay(_externalServicesConfiguration.OneOneOneUrl)
                .PreserveThreadContext();
        }

        private async Task ViewOnBackHomeRequested()
        {
            await _view.AppNavigation
                .PopToRoot()
                .PreserveThreadContext();
        }
    }
}