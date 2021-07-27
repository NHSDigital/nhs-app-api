using NHSOnline.App.Controls;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.WebIntegration.Models
{
    internal sealed class NhsLoginOnDemandGpSessionModel
    {
        public string AssertedLoginIdentity { get; }
        public string RedirectTo { get; }
        internal NavigationFooterItem FooterItem { get; }
        internal INhsAppNavigationHandler NavigationHandler { get; }

        public NhsLoginOnDemandGpSessionModel(
            string assertedLoginIdentity,
            string redirectTo,
            NavigationFooterItem footerItem,
            INhsAppNavigationHandler navigationHandler)
        {
            AssertedLoginIdentity = assertedLoginIdentity;
            RedirectTo = redirectTo;
            FooterItem = footerItem;
            NavigationHandler = navigationHandler;
        }
    }
}