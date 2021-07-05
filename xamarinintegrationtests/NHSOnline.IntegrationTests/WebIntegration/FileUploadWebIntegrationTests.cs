using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    public class FileUploadWebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanUploadTheirFileToTheTestProviderFileUploadScreenAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            AndroidTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToFileUpload();

            AndroidFileUploadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .UploadTestFile()
                .PageContent
                .AssertFileNotSelected()
                .UploadFile();

            AndroidStoragePage
                .AssertOnPage(driver)
                .SelectFile();

            AndroidFileUploadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.AssertFileSelected();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanUploadTheirFileToTheTestProviderFileUploadScreenIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            IOSMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            IOSTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToFileUpload();

            IOSFileUploadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .UploadTestFile()
                .PageContent.UploadFile();

            IOSFileSourceDialog
                .GetPanel(driver)
                .SelectBrowse();

            IOSStoragePage
                .AssertOnPage(driver)
                .SearchForText()
                .SelectFile();

            IOSFileUploadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.AssertFileSelected();
        }
    }
}