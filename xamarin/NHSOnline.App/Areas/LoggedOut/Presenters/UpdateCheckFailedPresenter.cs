using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class UpdateCheckFailedPresenter
    {
        private readonly IUpdateCheckFailedView _view;
        private readonly ILogger<UpdateCheckFailedPresenter> _logger;

        public UpdateCheckFailedPresenter(
            IUpdateCheckFailedView view,
            ILogger<UpdateCheckFailedPresenter> logger)
        {
            _view = view;
            _logger = logger;

            view.AppNavigation
                .RegisterHandler(BackToHomeRequested, (view, handler) => view.BackToHomeRequested = handler);
        }

        private async Task BackToHomeRequested()
        {
            _logger.LogInformation("Close Requested");
            await _view.AppNavigation.Pop().PreserveThreadContext();
        }
    }
}