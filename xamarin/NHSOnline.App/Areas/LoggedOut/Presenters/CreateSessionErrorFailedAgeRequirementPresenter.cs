using System;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class CreateSessionErrorFailedAgeRequirementPresenter
    {
        private readonly ICreateSessionErrorFailedAgeRequirementView _view;
        private readonly IAppBrowserTab _appBrowserTab;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public CreateSessionErrorFailedAgeRequirementPresenter(
            ICreateSessionErrorFailedAgeRequirementView view,
            CreateSessionErrorFailedAgeRequirementModel model,
            IAppBrowserTab appBrowserTab,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _appBrowserTab = appBrowserTab;
            _externalServicesConfiguration = externalServicesConfiguration;

            _view.ServiceDeskReference = model.ServiceDeskReference;

            _view.OneOneOneRequested += ViewOnOneOneOneRequested;
        }

        private async void ViewOnOneOneOneRequested(object sender, EventArgs e)
        {
            await _appBrowserTab
                .OpenAppBrowserTab(_externalServicesConfiguration.OneOneOneUrl)
                .PreserveThreadContext();
        }
    }
}