using NHSOnline.App.Areas.LoggedOut.Models;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class CreateSessionErrorPresenter
    {
        private readonly ICreateSessionErrorView _view;
        private readonly CreateSessionErrorModel _model;

        public CreateSessionErrorPresenter(
            ICreateSessionErrorView view,
            CreateSessionErrorModel model)
        {
            _view = view;
            _model = model;
        }
    }
}