namespace NHSOnline.App.Presenters
{
    internal sealed class MainPresenter
    {
        public MainPresenter(IMainView view)
        {
            View = view;
        }

        private IMainView View { get; }
    }
}
