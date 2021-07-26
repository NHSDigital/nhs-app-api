using NHSOnline.App.Controls;

namespace NHSOnline.App.Areas.WebIntegration.Models
{
    internal sealed class NhsLoginOnDemandGpSessionModel
    {
        public string AssertedLoginIdentity { get; }
        public string RedirectTo { get; }
        internal NavigationFooterItem FooterItem { get; }

        public NhsLoginOnDemandGpSessionModel(
            string assertedLoginIdentity,
            string redirectTo,
            NavigationFooterItem footerItem)
        {
            AssertedLoginIdentity = assertedLoginIdentity;
            RedirectTo = redirectTo;
            FooterItem = footerItem;
        }
    }
}