using System.Threading.Tasks;
using NHSOnline.App.Config;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class CreateSessionErrorFailedAgeRequirementPresenter
    {
        private readonly ICreateSessionErrorFailedAgeRequirementView _view;
        private readonly IBrowser _browser;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public CreateSessionErrorFailedAgeRequirementPresenter(
            ICreateSessionErrorFailedAgeRequirementView view,
            IBrowser browser,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _browser = browser;
            _externalServicesConfiguration = externalServicesConfiguration;

            _view.AppNavigation
                .RegisterHandler(ViewDigitalCovidPassRequested, (view, handler) => view.DigitalCovidPassRequested = handler)
                .RegisterHandler(ViewPaperCovidPassRequested, (view, handler) => view.PaperCovidPassRequested = handler)
                .RegisterHandler(ViewOnOneOneOneRequested, (view, handler) => view.OneOneOneRequested = handler);
        }

        private async Task ViewDigitalCovidPassRequested()
        {
            await _browser
                .OpenBrowserOverlay(_externalServicesConfiguration.DigitalCovidPassUrl)
                .PreserveThreadContext();
        }

        private async Task ViewPaperCovidPassRequested()
        {
            await _browser
                .OpenBrowserOverlay(_externalServicesConfiguration.PaperCovidPassUrl)
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
