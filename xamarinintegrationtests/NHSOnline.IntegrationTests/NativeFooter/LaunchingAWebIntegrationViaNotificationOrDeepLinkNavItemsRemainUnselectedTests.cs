using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.NativeFooter
{
    [TestClass]
    [BusinessRule("BR-NAV-02.10", "Accessing a web integration from a deep link, notification or claimed URL navigates to the web integration and clears any previously highlighted nav icons")]
    public class LaunchingAWebIntegrationViaNotificationOrDeepLinkNavItemsRemainUnselectedTests
    {
        [NhsAppManualTest("NHSO-14951", "Unable to automate sending notifications tests at the moment")]
        public void APatientSeesTheNavigationFooterDeselectedWhenNavigatingToAWebIntegrationViaNotification()
        {
        }

        [NhsAppManualTest("NHSO-14951", "Unable to automate deep link tests at the moment")]
        public void APatientSeesTheNavigationFooterDeselectedWhenNavigatingToAWebIntegrationViaDeepLink()
        {
        }

        [NhsAppManualTest("NHSO-14951", "Unable to automate claimed url tests at the moment")]
        public void APatientSeesTheNavigationFooterDeselectedWhenNavigatingToAWebIntegrationViaClaimedUrl()
        {
        }
    }
}