using System;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.WebIntegration.Models
{
    internal sealed class WebIntegrationModel
    {
        internal WebIntegrationModel(
            INhsAppNavigationHandler nhsAppPopToRootNavigationHandler,
            Uri url)
        {
            NavigationHandler = nhsAppPopToRootNavigationHandler;
            Url = url;
        }

        internal INhsAppNavigationHandler NavigationHandler { get; }
        internal Uri Url { get; }
    }
}