using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NHSOnline.App.Navigation
{
    internal interface IAppNavigation
    {
        Task Push(Page page);
        Task PushAnimated(Page page);
        Task Pop();
        Task PopAnimated();
        Task PopToRoot();
        Task PopToRootAnimated();
        Task PopToNewRoot(Page newRootPage);
        Task PopToNewRootAnimated(Page newRootPage);
        Task ReplaceCurrentPage(Page page);
    }

    internal interface IAppNavigation<out TEvents> : IAppNavigation
    {
        IAppNavigation<TEvents> RegisterHandler(Func<Task> handler, Action<TEvents, Func<Task>?> assignHandler);
        IAppNavigation<TEvents> RegisterHandler<TArgs>(Func<TArgs, Task> handler, Action<TEvents, Func<TArgs, Task>?> assignHandler);

        IAppNavigation<TEvents> RegisterPermanentHandler(Func<Task> handler, Action<TEvents, Func<Task>?> assignHandler);
        IAppNavigation<TEvents> RegisterPermanentHandler<TArgs>(Func<TArgs, Task> handler, Action<TEvents, Func<TArgs, Task>?> assignHandler);
    }
}