using System.Threading.Tasks;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class CreateSessionErrorFallbackPresenter
    {
        private readonly ICreateSessionErrorFallbackView _view;
        private readonly CreateSessionErrorFallbackModel _model;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public CreateSessionErrorFallbackPresenter(
            ICreateSessionErrorFallbackView view,
            CreateSessionErrorFallbackModel model,
            IBrowserOverlay browserOverlay,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _model = model;
            _browserOverlay = browserOverlay;
            _externalServicesConfiguration = externalServicesConfiguration;

            _view.AppNavigation
                .RegisterHandler(ViewOnOneOneOneRequested, (view, handler) => view.OneOneOneRequested = handler)
                .RegisterHandler(ViewOnContactUsRequested, (view, handler) => view.ContactUsRequested = handler)
                .RegisterHandler(ViewOnBackHomeRequested, (view, handler) => view.BackHomeRequested = handler);
        }

        private async Task ViewOnOneOneOneRequested()
        {
            await _browserOverlay
                .OpenBrowserOverlay(_externalServicesConfiguration.OneOneOneUrl)
                .PreserveThreadContext();
        }

        private async Task ViewOnContactUsRequested()
        {
            await _browserOverlay
                .OpenBrowserOverlay(_externalServicesConfiguration.NhsUkContactUsUrl)
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