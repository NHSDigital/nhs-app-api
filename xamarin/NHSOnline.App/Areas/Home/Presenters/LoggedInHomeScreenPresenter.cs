using NHSOnline.App.Areas.Home.Models;

namespace NHSOnline.App.Areas.Home.Presenters
{
    internal sealed class LoggedInHomeScreenPresenter
    {
        private readonly ILoggedInHomeScreenView _view;
        private readonly LoggedInHomeScreenModel _model;

        public LoggedInHomeScreenPresenter(
            ILoggedInHomeScreenView view,
            LoggedInHomeScreenModel model)
        {
            _view = view;
            _model = model;
        }
    }
}