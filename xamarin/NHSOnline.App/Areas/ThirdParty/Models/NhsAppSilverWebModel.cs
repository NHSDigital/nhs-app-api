using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.ThirdParty.Models
{
    internal sealed class NhsAppSilverWebModel
    {
        internal NhsAppSilverWebModel(
            INhsAppNavigationHandler nhsAppPopToRootNavigationHandler,
            string silverUrl)
        {
            NavigationHandler = nhsAppPopToRootNavigationHandler;
            SilverUrl = silverUrl;
        }

        internal INhsAppNavigationHandler NavigationHandler { get; }

        internal string SilverUrl { get; }
    }
}