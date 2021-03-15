using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas;
using NHSOnline.App.Logging;
using Xamarin.Forms;

namespace NHSOnline.App.Navigation
{
    internal sealed class AppNavigation<TEvents>: IAppNavigation<TEvents>
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger<AppNavigation<TEvents>>();

        private readonly List<Action> _enableHandlers = new List<Action>();
        private readonly List<Action> _suppressHandlers = new List<Action>();

        private readonly TEvents _viewEvents;
        private readonly INavigation _navigation;

        public AppNavigation(TEvents viewEvents, INavigation navigation)
        {
            _viewEvents = viewEvents;
            _navigation = navigation;
        }

        public IAppNavigation<TEvents> RegisterHandler(Func<Task> handler, Action<TEvents, Func<Task>?> assignHandler)
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

        public IAppNavigation<TEvents> RegisterHandler<TArgs>(Func<TArgs, Task> handler, Action<TEvents, Func<TArgs, Task>?> assignHandler)
        {
            _enableHandlers.Add(() => assignHandler(_viewEvents, handler));
            _suppressHandlers.Add(() => assignHandler(_viewEvents, SuppressedEvent));
            return this;
        }

        private static Task SuppressedEvent<TArgs>(TArgs arg)
        {
            Logger.LogInformation("Suppressing event ({TArgs})", typeof(TArgs).Name);
            return Task.CompletedTask;
        }

        public void EnableHandlers()
        {
            _enableHandlers.ForEach(enable => enable());
        }

        public void SuppressHandlers()
        {
            _suppressHandlers.ForEach(suppress => suppress());
        }

        public async Task Push(Page page)
        {
            SuppressHandlers();

            await _navigation.PushAsync(page).PreserveThreadContext();
        }

        public async Task Pop()
        {
            SuppressHandlers();

            await _navigation.PopAsync().PreserveThreadContext();
        }

        public async Task PopToNewRoot(Page newRootPage)
        {
            SuppressHandlers();

            _navigation.InsertPageBefore(newRootPage, _navigation.NavigationStack[0]);
            await _navigation.PopToRootAsync().PreserveThreadContext();
        }

        public async Task ReplaceCurrentPage(Page page)
        {
            SuppressHandlers();

            var currentPage = _navigation.NavigationStack[^1];

            await _navigation.PushAsync(page).PreserveThreadContext();

            _navigation.RemovePage(currentPage);
        }
    }
}