using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.Home.Presenters
{
    internal sealed class NhsAppPopToRootNavigationHandler : INhsAppNavigationHandler
    {
        private readonly IAppNavigation<INhsAppWebView.IEvents> _appNavigation;
        private readonly INhsAppNavigationHandler _rootHandler;

        public NhsAppPopToRootNavigationHandler(
            INhsAppNavigationHandler rootHandler,
            IAppNavigation<INhsAppWebView.IEvents> navigation)
        {
            _rootHandler = rootHandler;
            _appNavigation = navigation;
        }

        public async Task HomeRequested()
        {
            await _appNavigation.PopToRoot().PreserveThreadContext();
            await _rootHandler.HomeRequested().PreserveThreadContext();
        }

        public async Task AdviceRequested()
        {
            await _appNavigation.PopToRoot().PreserveThreadContext();
            await _rootHandler.AdviceRequested().PreserveThreadContext();
        }

        public async Task AppointmentsRequested()
        {
            await _appNavigation.PopToRoot().PreserveThreadContext();
            await _rootHandler.AppointmentsRequested().PreserveThreadContext();
        }

        public async Task PrescriptionsRequested()
        {
            await _appNavigation.PopToRoot().PreserveThreadContext();
            await _rootHandler.PrescriptionsRequested().PreserveThreadContext();
        }

        public async Task YourHealthRequested()
        {
            await _appNavigation.PopToRoot().PreserveThreadContext();
            await _rootHandler.YourHealthRequested().PreserveThreadContext();
        }

        public async Task MoreRequested()
        {
            await _appNavigation.PopToRoot().PreserveThreadContext();
            await _rootHandler.MoreRequested().PreserveThreadContext();
        }

        public async Task MessagesRequested()
        {
            await _appNavigation.PopToRoot().PreserveThreadContext();
            await _rootHandler.MessagesRequested().PreserveThreadContext();
        }

        public async Task GoToNhsAppPageRequested(string page)
        {
            await _appNavigation.PopToRootAnimated().PreserveThreadContext();
            await _rootHandler.GoToNhsAppPageRequested(page).PreserveThreadContext();
        }

        public async Task RedirectToDeepLinkRequested(Uri deeplinkUrl)
        {
            await _appNavigation.PopToRootAnimated().PreserveThreadContext();
            await _rootHandler.RedirectToDeepLinkRequested(deeplinkUrl).PreserveThreadContext();
        }

        public async Task NavigateToOnDemandGpReturn(Dictionary<string, string> queryParameters)
        {
            await _appNavigation.PopToRoot().PreserveThreadContext();
            await _rootHandler.NavigateToOnDemandGpReturn(queryParameters).PreserveThreadContext();
        }
    }
}
