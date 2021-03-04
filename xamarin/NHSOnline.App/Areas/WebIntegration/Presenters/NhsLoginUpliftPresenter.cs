using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.WebIntegration.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Services;
using NHSOnline.App.Services.Media;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration.Presenters
{
    internal sealed class NhsLoginUpliftPresenter
    {
        private readonly ILogger _logger;
        private readonly INhsLoginUpliftView _view;
        private readonly NhsLoginUpliftModel _model;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly WebIntegrationUriDestination _uriDestination;
        private readonly ISelectMediaService _selectMediaService;

        public NhsLoginUpliftPresenter(
            ILogger<NhsLoginUpliftPresenter> logger,
            INhsLoginUpliftView view,
            NhsLoginUpliftModel model,
            INhsLoginConfiguration nhsLoginConfiguration,
            IBrowserOverlay browserOverlay,
            ISelectMediaService selectMediaService)
        {
            _logger = logger;
            _view = view;
            _model = model;
            _browserOverlay = browserOverlay;
            _selectMediaService = selectMediaService;

            _uriDestination = new WebIntegrationUriDestination(nhsLoginConfiguration, model.Url);

            _view.Appearing = ViewOnAppearing;
            _view.Navigating = ViewOnNavigating;
            _view.BackRequested = BackRequested;
            _view.SelectMediaRequested = SelectMediaRequested;
        }

        private async Task SelectMediaRequested(ISelectMediaRequest request)
        {
            await _selectMediaService.SelectMedia(request).PreserveThreadContext();
        }

        private Task ViewOnAppearing()
        {
            _view.Appearing = null;
            _view.GoToUri(_model.Url);

            return Task.CompletedTask;
        }

        private async Task ViewOnNavigating(WebNavigatingEventArgs webNavigatingEventArgs)
        {
            var url = new Uri(webNavigatingEventArgs.Url);
            if (_uriDestination.ShouldOpenInBrowserOverlay(url))
            {
                await OpenInBrowserOverlay(webNavigatingEventArgs, url).PreserveThreadContext();
            }
        }

        private async Task OpenInBrowserOverlay(WebNavigatingEventArgs webNavigatingEventArgs, Uri url)
        {
            webNavigatingEventArgs.Cancel = true;
            await _browserOverlay.OpenBrowserOverlay(url).PreserveThreadContext();
        }

        private async Task BackRequested()
        {
            _logger.LogInformation("Back Requested");
            await _view.Navigation.PopAsync().PreserveThreadContext();
        }
    }
}