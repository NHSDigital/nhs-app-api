using System;
using NHSOnline.App.Controls;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.Errors.Models
{
    internal class FullNavigationTryAgainFileDownloadErrorModel
    {
        internal INhsAppNavigationHandler NavigationHandler { get; }
        internal NavigationFooterItem SelectedFooterItem { get; }
        public Uri? HelpUrl { get; }

        internal FullNavigationTryAgainFileDownloadErrorModel(
            INhsAppNavigationHandler navigationHandler,
            NavigationFooterItem navigationFooterItem,
            Uri helpUrl)
        {
            NavigationHandler = navigationHandler;
            SelectedFooterItem = navigationFooterItem;
            HelpUrl = helpUrl;
        }

        internal FullNavigationTryAgainFileDownloadErrorModel(
            INhsAppNavigationHandler navigationHandler,
            NavigationFooterItem navigationFooterItem)
        {
            NavigationHandler = navigationHandler;
            SelectedFooterItem = navigationFooterItem;
        }
    }
}