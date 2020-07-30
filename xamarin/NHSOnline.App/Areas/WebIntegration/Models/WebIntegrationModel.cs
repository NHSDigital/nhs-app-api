using System;
using NHSOnline.App.Controls.WebViews.KnownServices;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.WebIntegration.Models
{
    internal sealed class WebIntegrationModel
    {
        internal WebIntegrationModel(
            INhsAppNavigationHandler nhsAppPopToRootNavigationHandler,
            Uri url,
            MenuTab menuTab)
        {
            NavigationHandler = nhsAppPopToRootNavigationHandler;
            Url = url;
            MenuTab = menuTab;
        }

        internal INhsAppNavigationHandler NavigationHandler { get; }
        internal Uri Url { get; }
        internal MenuTab MenuTab { get; }
    }
}