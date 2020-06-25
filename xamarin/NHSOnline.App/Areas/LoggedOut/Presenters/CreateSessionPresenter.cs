using NHSOnline.App.Areas.LoggedOut.Models;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class CreateSessionPresenter
    {
        private readonly ICreateSessionView _view;
        private readonly CreateSessionModel _model;

        public CreateSessionPresenter(
            ICreateSessionView view,
            CreateSessionModel model)
        {
            _view = view;
            _model = model;
        }
    }
}