using System;
using NHSOnline.App.Controls;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.WebIntegration.Models
{
    internal sealed class WebIntegrationModel
    {
        internal WebIntegrationModel(
            INhsAppNavigationHandler navigationHandler,
            Uri url,
            NavigationFooterItem footerItem)
        {
            NavigationHandler = navigationHandler;
            Url = url;
            FooterItem = footerItem;
        }

        internal INhsAppNavigationHandler NavigationHandler { get; }
        internal Uri Url { get; }
        internal NavigationFooterItem FooterItem { get; }
    }
}