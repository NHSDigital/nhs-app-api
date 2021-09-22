using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    [BusinessRule("BR-LOG-08.3", "Log in for a user with a problem with their ODS, GPSS or NHS number displays a shutter screen")]
    public class CreateSessionOdsCodeNotSupportedErrorTests
    {
        [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenPatientHasUnknownOdsCodeAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.UnknownOdsCode);

            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidCreateSessionOdsCodeNotSupportedErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenPatientHasUnknownOdsCodeIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.UnknownOdsCode);

            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSCreateSessionOdsCodeNotSupportedErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenPatientHasUnknownSupplierAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.NoOdsCode);
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidCreateSessionOdsCodeNotFoundErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenPatientHasUnknownSupplierIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.NoOdsCode);
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSCreateSessionOdsCodeNotFoundErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenPatientHasNoNhsNumberAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithNhsNumber(NhsNumber.None)
                .WithProofLevel5();
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidCreateSessionNoNhsNumberErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenPatientHasNoNhsNumberIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithNhsNumber(NhsNumber.None)
                .WithProofLevel5();
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSCreateSessionNoNhsNumberErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }
    }
}