using System;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.WebIntegration.Models
{
    internal sealed class NhsLoginUpliftModel
    {
        internal Uri Url { get; }
        internal INhsAppNavigationHandler NavigationHandler { get; }

        internal NhsLoginUpliftModel(
            Uri url,
            INhsAppNavigationHandler navigationHandler)
        {
            Url = url;
            NavigationHandler = navigationHandler;
        }
    }
}