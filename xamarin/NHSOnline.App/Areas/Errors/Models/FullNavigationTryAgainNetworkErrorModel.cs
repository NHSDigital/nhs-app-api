using NHSOnline.App.Controls;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.Errors.Models
{
    internal class FullNavigationTryAgainNetworkErrorModel
    {
        internal INhsAppNavigationHandler NavigationHandler { get; }
        internal ITryAgainWebview TryAgainWebview { get; }
        internal NavigationFooterItem SelectedFooterItem { get; }

        internal FullNavigationTryAgainNetworkErrorModel(
            INhsAppNavigationHandler navigationHandler,
            ITryAgainWebview tryAgainWebview,
            NavigationFooterItem navigationFooterItem)
        {
            NavigationHandler = navigationHandler;
            TryAgainWebview = tryAgainWebview;
            SelectedFooterItem = navigationFooterItem;
        }
    }
}



