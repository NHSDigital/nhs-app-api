using System;
using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class LoggedOutHomeScreenPresenter
    {
        private readonly ILogger<LoggedOutHomeScreenPresenter> _logger;

        public LoggedOutHomeScreenPresenter(
            ILoggedOutHomeScreenView view,
            ILogger<LoggedOutHomeScreenPresenter> logger)
        {
            _logger = logger;

            view.LoginRequested += ViewOnLoginRequested;
        }

        private void ViewOnLoginRequested(object sender, EventArgs e)
        {
            _logger.LogInformation("Login Requested");
        }
    }
}
