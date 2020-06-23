using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Presenters
{
    internal sealed class MainPresenter
    {
        public MainPresenter(IMainView view, ILogger<MainPresenter> logger)
        {
            View = view;
            logger.LogInformation("Hello!");
        }

        private IMainView View { get; }
    }
}
