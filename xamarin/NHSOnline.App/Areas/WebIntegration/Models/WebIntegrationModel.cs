using System;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.WebIntegration.Models
{
    internal sealed class WebIntegrationModel
    {
        internal WebIntegrationModel(
            INhsAppNavigationHandler navigationHandler,
            Uri url)
        {
            NavigationHandler = navigationHandler;
            Url = url;
        }

        internal INhsAppNavigationHandler NavigationHandler { get; }
        internal Uri Url { get; }
    }
}