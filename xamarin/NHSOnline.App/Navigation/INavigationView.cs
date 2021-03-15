namespace NHSOnline.App.Navigation
{
    internal interface INavigationView<out TEvents>
    {
        internal IAppNavigation<TEvents> AppNavigation { get; }
    }
}