using System;
using System.Collections.ObjectModel;
using NHSOnline.App.Controls;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.WebIntegration.Models
{
    internal sealed class WebIntegrationModel
    {
        internal WebIntegrationModel(
            INhsAppNavigationHandler navigationHandler,
            Uri url,
            NavigationFooterItem footerItem,
            Collection<Uri> additionalDomains)
        {
            NavigationHandler = navigationHandler;
            Url = url;
            FooterItem = footerItem;
            AdditionalDomains = additionalDomains;
        }

        internal INhsAppNavigationHandler NavigationHandler { get; }
        internal Uri Url { get; }
        internal NavigationFooterItem FooterItem { get; }
        internal Collection<Uri> AdditionalDomains { get; }
    }
}