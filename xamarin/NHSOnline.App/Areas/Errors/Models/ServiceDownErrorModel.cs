using NHSOnline.App.Controls;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.Errors.Models
{
    internal class ServiceDownErrorModel
    {
        internal INhsAppNavigationHandler NavigationHandler { get; }
        internal NavigationFooterItem SelectedFooterItem { get; }

        internal ServiceDownErrorModel(
            INhsAppNavigationHandler navigationHandler,
            NavigationFooterItem navigationFooterItem)
        {
            NavigationHandler = navigationHandler;
            SelectedFooterItem = navigationFooterItem;
        }
    }
}