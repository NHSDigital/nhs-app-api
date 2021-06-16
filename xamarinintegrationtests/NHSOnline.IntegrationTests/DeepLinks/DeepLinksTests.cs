using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.DeepLinks
{
    [TestClass]
    public class DeepLinksTests
     {
         [NhsAppAndroidTest(OSVersion = AndroidOSVersion.Ten, AndroidDevice = AndroidDevice.Pixel4)]
         public void BackgroundAppDeepLink(IAndroidDriverWrapper driver)
         {
             var patient = new EmisPatient()
                 .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
             using var patients = Mocks.Patients.Add(patient);

             LoginProcess.LogAndroidPatientIn(driver, patient);

             AndroidLoggedInHomePage
                 .AssertOnPage(driver);

             driver.BackgroundApp();

             driver.OpenChromeApp()
                 .NavigateToDeepLinkLauncher();

             AndroidDeepLinkLauncherPage
                 .AssertOnPage(driver)
                 .ClickLink();

             AndroidDeepLinkAppChoice
                 .AssertDisplayed(driver)
                 .ChooseNhsApp();

             AndroidAppointmentsPage.AssertOnPage(driver);
         }

         [NhsAppAndroidTest]
         public void ClosedAppDeepLink(IAndroidDriverWrapper driver)
         {
             var patient = new EmisPatient()
                 .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
             using var patients = Mocks.Patients.Add(patient);

             driver.CloseApp();

             driver.OpenChromeApp()
                 .NavigateToDeepLinkLauncher();

             AndroidDeepLinkLauncherPage
                 .AssertOnPage(driver)
                 .ClickLink();

             AndroidDeepLinkAppChoice
                 .AssertDisplayed(driver)
                 .ChooseNhsApp();

             LoginProcess.LogAndroidPatientIn(driver, patient);
             AndroidAppointmentsPage.AssertOnPage(driver);
         }

    }
}