using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.Errors.Models;
using NHSOnline.App.Areas.WebIntegration.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.Services;
using NHSOnline.App.Services.Media;
using NHSOnline.App.Threading;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration.Presenters
{
    internal sealed class NhsLoginUpliftPresenter
    {
        private bool _hasAlreadyAppeared;

        private readonly ILogger _logger;
        private readonly INhsLoginUpliftView _view;
        private readonly NhsLoginUpliftModel _model;
        private readonly IPageFactory _pageFactory;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly WebIntegrationUriDestination _uriDestination;
        private readonly ISelectMediaService _selectMediaService;

        public NhsLoginUpliftPresenter(
            ILogger<NhsLoginUpliftPresenter> logger,
            INhsLoginUpliftView view,
            NhsLoginUpliftModel model,
            IPageFactory pageFactory,
            INhsLoginConfiguration nhsLoginConfiguration,
            IBrowserOverlay browserOverlay,
            ISelectMediaService selectMediaService)
        {
            _logger = logger;
            _view = view;
            _model = model;
            _pageFactory = pageFactory;
            _browserOverlay = browserOverlay;
            _selectMediaService = selectMediaService;

            _uriDestination = new WebIntegrationUriDestination(nhsLoginConfiguration, model.Url, new Collection<Uri>());

            _view.AppNavigation
                .RegisterHandler(ViewOnAppearing, (view, handler) => view.Appearing = handler)
                .RegisterHandler<WebNavigatingEventArgs>(ViewOnNavigating, (view, handler) => view.Navigating = handler)
                .RegisterHandler(ViewOnNavigationFailed, (view, handler) => view.NavigationFailed = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler)
                .RegisterHandler<ISelectMediaRequest>(SelectMediaRequested, (view, handler) => view.SelectMediaRequested = handler);
        }

        private async Task SelectMediaRequested(ISelectMediaRequest request)
        {
            await _selectMediaService.SelectMedia(request).PreserveThreadContext();
        }

        private Task ViewOnAppearing()
        {
            if (_hasAlreadyAppeared)
            {
                return Task.CompletedTask;
            }

            _hasAlreadyAppeared = true;

            _view.GoToUri(_model.Url);

            return Task.CompletedTask;
        }

        private void ViewOnNavigating(WebNavigatingEventArgs webNavigatingEventArgs)
        {
            var url = new Uri(webNavigatingEventArgs.Url);
            if (_uriDestination.ShouldOpenInBrowserOverlay(url))
            {
                webNavigatingEventArgs.Cancel = true;
                NhsAppResilience.ExecuteOnMainThread(() =>
                {
                    _browserOverlay.OpenBrowserOverlay(url).PreserveThreadContext();
                });
            }
        }

        private Task ViewOnNavigationFailed()
        {
            var model = new CloseSlimBackToHomeNetworkErrorModel();
            var page = _pageFactory.CreatePageFor(model);
            return _view.AppNavigation.Push(page);
        }

        private async Task BackRequested()
        {
            _logger.LogInformation("Back Requested");
            await _view.AppNavigation.Pop().PreserveThreadContext();
        }
    }
}