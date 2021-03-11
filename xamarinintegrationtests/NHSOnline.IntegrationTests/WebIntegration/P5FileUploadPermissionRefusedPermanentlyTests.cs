using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    [BusinessRule("BR-LOG-12.8", "Rejecting the NHS app request for appropriate permissions in the NHS login uplift journey for file upload dismisses the native alert")]
    public class P5FileUploadPermissionRefusedPermanentlyTests
    {
        [NhsAppManualTest("NHSO-13698",
            "BrowserStack only shows the permissions dialog once on Android and this does not have a 'Don't ask me again' option")]
        public void APatientWithProofLevelFivePermanentlyRefusesStoragePermissionsAndroid()
        {

        }

        [NhsAppManualTest("NHSO-13698", "BrowserStack does not show the permissions dialog for iOS")]
        public void APatientWithProofLevelFivePermanentlyRefusesStoragePermissionsIOS()
        {

        }
    }
}