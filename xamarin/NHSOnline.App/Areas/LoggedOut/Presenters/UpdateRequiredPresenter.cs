using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Services.ForcedUpdate;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class UpdateRequiredPresenter
    {
        private readonly ILogger<UpdateRequiredPresenter> _logger;
        private readonly IForcedUpdateCheckService _forcedUpdateCheckService;

        public UpdateRequiredPresenter(
            ILogger<UpdateRequiredPresenter> logger,
            IUpdateRequiredView view,
            IForcedUpdateCheckService forcedUpdateCheckService)
        {
            _logger = logger;
            _forcedUpdateCheckService = forcedUpdateCheckService;

            view.AppNavigation
                .RegisterHandler(OpenAppStoreUrlRequested, (view, handler) => view.OpenAppStoreUrlRequested = handler);
        }

        private async Task OpenAppStoreUrlRequested()
        {
            _logger.LogInformation("Open app store url requested");
            await _forcedUpdateCheckService.OpenAppStoreUrl().PreserveThreadContext();
        }
    }
}