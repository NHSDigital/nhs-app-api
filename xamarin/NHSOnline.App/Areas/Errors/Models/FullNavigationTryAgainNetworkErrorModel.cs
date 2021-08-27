using System;
using NHSOnline.App.Controls;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.Errors.Models
{
    internal class FullNavigationTryAgainNetworkErrorModel
    {
        internal INhsAppNavigationHandler NavigationHandler { get; }
        internal NavigationFooterItem SelectedFooterItem { get; }
        public Action RetryAction { get; }

        internal FullNavigationTryAgainNetworkErrorModel(
            INhsAppNavigationHandler navigationHandler,
            NavigationFooterItem navigationFooterItem,
            Action retryAction)
        {
            NavigationHandler = navigationHandler;
            SelectedFooterItem = navigationFooterItem;
            RetryAction = retryAction;
        }
    }
}