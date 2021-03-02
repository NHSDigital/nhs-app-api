using System;
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

            _view.OneOneOneRequested += ViewOnOneOneOneRequested;
            _view.ContactUsRequested += ViewOnContactUsRequested;
            _view.BackHomeRequested += ViewOnBackHomeRequested;
        }

        private async void ViewOnOneOneOneRequested(object sender, EventArgs e)
        {
            await _browserOverlay
                .OpenBrowserOverlay(_externalServicesConfiguration.OneOneOneUrl)
                .PreserveThreadContext();
        }

        private async void ViewOnContactUsRequested(object sender, EventArgs e)
        {
            await _browserOverlay
                .OpenBrowserOverlay(_externalServicesConfiguration.NhsUkContactUsUrl)
                .PreserveThreadContext();
        }

        private async void ViewOnBackHomeRequested(object sender, EventArgs e)
        {
            await _view.Navigation
                .PopToRootAsync()
                .PreserveThreadContext();
        }
    }
}