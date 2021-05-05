using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Navigation
{

    internal interface INavigationView
    {
        Task HandleDeeplink(Uri deeplinkUrl);
    }

    internal interface INavigationView<out TEvents>: INavigationView
    {
        internal IAppNavigation<TEvents> AppNavigation { get; }

    }
}