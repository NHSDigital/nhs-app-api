using System;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberPresenter
    {
        private readonly ICreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberView _view;
        private readonly IAppBrowserTab _appBrowserTab;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberPresenter(
            ICreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberView view,
            CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberModel model,
            IAppBrowserTab appBrowserTab,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _appBrowserTab = appBrowserTab;
            _externalServicesConfiguration = externalServicesConfiguration;

            _view.ServiceDeskReference = model.ServiceDeskReference;

            _view.MyHealthOnlineRequested += ViewOnMyHealthOnlineRequested;
            _view.OneOneOneWalesRequested += ViewOnOneOneOneWalesRequested;
            _view.OneOneOneRequested += ViewOnOneOneOneRequested;
            _view.ContactUsRequested += ViewOnContactUsRequested;
        }

        private async void ViewOnMyHealthOnlineRequested(object sender, EventArgs e)
        {
            await _appBrowserTab
                .OpenAppBrowserTab(_externalServicesConfiguration.MyHealthOnlineUrl)
                .PreserveThreadContext();
        }

        private async void ViewOnOneOneOneWalesRequested(object sender, EventArgs e)
        {
            await _appBrowserTab
                .OpenAppBrowserTab(_externalServicesConfiguration.OneOneOneWalesUrl)
                .PreserveThreadContext();
        }

        private async void ViewOnOneOneOneRequested(object sender, EventArgs e)
        {
            await _appBrowserTab
                .OpenAppBrowserTab(_externalServicesConfiguration.OneOneOneUrl)
                .PreserveThreadContext();
        }

        private async void ViewOnContactUsRequested(object sender, EventArgs e)
        {
            await _appBrowserTab
                .OpenAppBrowserTab(_externalServicesConfiguration.NhsUkContactUsUrl)
                .PreserveThreadContext();
        }
    }
}