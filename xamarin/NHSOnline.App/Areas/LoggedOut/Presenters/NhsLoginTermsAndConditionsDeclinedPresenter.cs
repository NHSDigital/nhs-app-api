using System;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class NhsLoginTermsAndConditionsDeclinedPresenter
    {
        private readonly INhsLoginTermsAndConditionsDeclinedView _view;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public NhsLoginTermsAndConditionsDeclinedPresenter(
            INhsLoginTermsAndConditionsDeclinedView view,
            IBrowserOverlay browserOverlay,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _browserOverlay = browserOverlay;
            _externalServicesConfiguration = externalServicesConfiguration;

            _view.BackToHomeRequested += ViewOnBackToHomeRequested;
            _view.OneOneOneRequested += ViewOnOneOneOneRequested;
        }

        private async void ViewOnBackToHomeRequested(object sender, EventArgs e)
        {
            await _view.Navigation
                .PopToRootAsync()
                .PreserveThreadContext();
        }

        private async void ViewOnOneOneOneRequested(object sender, EventArgs e)
        {
            var oneOneOneUri = _externalServicesConfiguration.OneOneOneUrl;
            await _browserOverlay
                .OpenBrowserOverlay(oneOneOneUri)
                .PreserveThreadContext();
        }
    }
}