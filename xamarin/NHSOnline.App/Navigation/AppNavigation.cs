using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices.Navigation;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using Xamarin.Forms;

namespace NHSOnline.App.Navigation
{
    internal sealed class AppNavigation<TEvents>: IAppNavigation<TEvents>
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger<AppNavigation<TEvents>>();

        private readonly List<Action> _enableHandlers = new List<Action>();
        private readonly List<Action> _suppressHandlers = new List<Action>();

        private readonly TEvents _viewEvents;
        private readonly INavigationService _navigationService;

        public AppNavigation(TEvents viewEvents, INavigationService navigationService)
        {
            _viewEvents = viewEvents;
            _navigationService = navigationService;
        }

        public IAppNavigation<TEvents> RegisterHandler(
            Func<Task> handler,
            Action<TEvents, Func<Task>?> assignHandler)
        {
            _enableHandlers.Add(() => assignHandler(_viewEvents, handler));
            _suppressHandlers.Add(() => assignHandler(_viewEvents, SuppressedEvent));
            return this;
        }

        private static Task SuppressedEvent()
        {
            Logger.LogInformation("Suppressing event");
            return Task.CompletedTask;
        }

        public IAppNavigation<TEvents> RegisterHandler<TArgs>(
            Func<TArgs, Task> handler,
            Action<TEvents, Func<TArgs, Task>?> assignHandler)
        {
            _enableHandlers.Add(() => assignHandler(_viewEvents, handler));
            _suppressHandlers.Add(() => assignHandler(_viewEvents, SuppressedEventAsync));
            return this;
        }

        public IAppNavigation<TEvents> RegisterHandler<TArgs>(Action<TArgs> handler, Action<TEvents, Action<TArgs>?> assignHandler)
        {
            _enableHandlers.Add(() => assignHandler(_viewEvents, handler));
            _suppressHandlers.Add(() => assignHandler(_viewEvents, SuppressedEventSync));
            return this;
        }

        public IAppNavigation<TEvents> RegisterPermanentHandler(Func<Task> handler, Action<TEvents, Func<Task>?> assignHandler)
        {
            assignHandler(_viewEvents, handler);
            return this;
        }

        public IAppNavigation<TEvents> RegisterPermanentHandler<TArgs>(Func<TArgs, Task> handler, Action<TEvents, Func<TArgs, Task>?> assignHandler)
        {
            assignHandler(_viewEvents, handler);
            return this;
        }

        private static Task SuppressedEventAsync<TArgs>(TArgs arg)
        {
            Logger.LogInformation("Suppressing event ({TArgs})", typeof(TArgs).Name);
            return Task.CompletedTask;
        }

        private static void SuppressedEventSync<TArgs>(TArgs arg) =>
            Logger.LogInformation("Suppressing event ({TArgs})", typeof(TArgs).Name);

        public void EnableHandlers() => _enableHandlers.ForEach(enable => enable());

        public void SuppressHandlers() => _suppressHandlers.ForEach(suppress => suppress());

        public async Task Push(Page page)
        {
            SuppressHandlers();
            await _navigationService.Push(page).PreserveThreadContext();
        }

        public async Task Pop()
        {
            SuppressHandlers();
            await _navigationService.Pop().PreserveThreadContext();
        }

        public async Task PopToRoot()
        {
            SuppressHandlers();
            await _navigationService.PopToRoot().PreserveThreadContext();
        }

        public async Task PopToNewRoot(Page newRootPage)
        {
            SuppressHandlers();
            await _navigationService.PopToNewRoot(newRootPage).PreserveThreadContext();
        }

        public async Task ReplaceCurrentPage(Page page)
        {
            SuppressHandlers();
            await _navigationService.ReplaceCurrentPage(page).PreserveThreadContext();
        }
    }
}