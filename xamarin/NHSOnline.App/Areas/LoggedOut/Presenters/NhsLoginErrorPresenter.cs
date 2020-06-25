using NHSOnline.App.Areas.LoggedOut.Models;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class NhsLoginErrorPresenter
    {
        private readonly INhsLoginErrorView _view;
        private readonly NhsLoginErrorModel _model;

        public NhsLoginErrorPresenter(
            INhsLoginErrorView view,
            NhsLoginErrorModel model)
        {
            _view = view;
            _model = model;
        }
    }
}