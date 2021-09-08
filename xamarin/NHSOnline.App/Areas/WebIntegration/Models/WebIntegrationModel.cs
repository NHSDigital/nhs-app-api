using System;
using System.Collections.Generic;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.WebIntegration.Models
{
    internal sealed class WebIntegrationModel
    {
        internal WebIntegrationModel(
            INhsAppNavigationHandler navigationHandler,
            NavigationFooterItem footerItem,
            WebIntegrationRequest webIntegrationRequest,
            IReadOnlyCollection<Uri>? additionalDomains,
            Uri helpUrl)
        {
            NavigationHandler = navigationHandler;
            FooterItem = footerItem;
            WebIntegrationRequest = webIntegrationRequest;
            AdditionalDomains = additionalDomains;
            HelpUrl = helpUrl;
        }

        internal INhsAppNavigationHandler NavigationHandler { get; }
        internal NavigationFooterItem FooterItem { get; }
        public WebIntegrationRequest WebIntegrationRequest { get; }
        public IReadOnlyCollection<Uri>? AdditionalDomains { get; }
        public Uri HelpUrl { get; }
    }
}