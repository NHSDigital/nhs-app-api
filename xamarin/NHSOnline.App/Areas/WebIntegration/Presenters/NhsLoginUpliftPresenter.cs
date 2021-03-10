using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NHSOnline.App.Areas.WebIntegration.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Controls.WebViews.Payloads.Paycasso;
using NHSOnline.App.DependencyServices.Paycasso;
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
        private readonly IPaycasso _paycasso;

        public NhsLoginUpliftPresenter(
            ILogger<NhsLoginUpliftPresenter> logger,
            INhsLoginUpliftView view,
            NhsLoginUpliftModel model,
            INhsLoginConfiguration nhsLoginConfiguration,
            IBrowserOverlay browserOverlay,
            ISelectMediaService selectMediaService,
            IPaycasso paycasso)
        {
            _logger = logger;
            _view = view;
            _model = model;
            _browserOverlay = browserOverlay;
            _selectMediaService = selectMediaService;
            _paycasso = paycasso;

            _uriDestination = new WebIntegrationUriDestination(nhsLoginConfiguration, model.Url);

            _view.Appearing = ViewOnAppearing;
            _view.Navigating = ViewOnNavigating;
            _view.BackRequested = BackRequested;
            _view.SelectMediaRequested = SelectMediaRequested;
            _view.LaunchPaycassoRequested = LaunchPaycassoRequested;
        }

        private async Task SelectMediaRequested(ISelectMediaRequest request)
        {
            await _selectMediaService.SelectMedia(request).PreserveThreadContext();
        }

        private async Task LaunchPaycassoRequested(LaunchPaycassoRequest request)
        {
            var result = await LaunchPaycasso(request).PreserveThreadContext();

            await result.Accept(new PaycassoResultVisitor(_view)).PreserveThreadContext();
        }

        private async Task<PaycassoResult> LaunchPaycasso(LaunchPaycassoRequest request)
        {
            PaycassoResult result;
            try
            {
                result = await _paycasso.Launch(request).PreserveThreadContext();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failure launching Paycasso");
                result = new PaycassoResult.Failure(e.Message);
            }
            return result;
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

        private sealed class PaycassoResultVisitor : IPaycassoResultVisitor
        {
            private static readonly JsonSerializerSettings JsonSerializerSettings = CreateJsonSerializerSettings();

            private readonly INhsLoginUpliftView _view;

            public PaycassoResultVisitor(INhsLoginUpliftView view) => _view = view;

            public async Task Visit(PaycassoResult.Success success)
            {
                var json = SerialiseResponse(success.Response);
                await _view.PaycassoOnSuccess(json).PreserveThreadContext();
            }

            public async Task Visit(PaycassoResult.Failure failure)
            {
                var json = SerialiseResponse(failure.Response);
                await _view.PaycassoOnFailure(json).PreserveThreadContext();
            }

            private static string SerialiseResponse<TResponse>(TResponse response)
            {
                return JsonConvert.SerializeObject(response, JsonSerializerSettings);
            }

            private static JsonSerializerSettings CreateJsonSerializerSettings()
            {
                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                };
                jsonSerializerSettings.Converters.Add(new StringEnumConverter());
                return jsonSerializerSettings;
            }
        }
    }
}