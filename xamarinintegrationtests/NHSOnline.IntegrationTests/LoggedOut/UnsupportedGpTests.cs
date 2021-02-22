using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    [BusinessRule("BR-LOG-08.9", "Log in for a user with a problem with their ODS, GPSS or NHS number displays a shutter screen")]
    public class UnsupportedGpTests
    {
        [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenPatientHasUnknownOdsCodeAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P9Patient()
                .WithUnknownOdsCode();
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidCreateSessionOdsCodeNotSupportedOrNoNhsNumberErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenPatientHasUnknownOdsCodeIos(IIOSDriverWrapper driver)
        {
            var patient = new P9Patient()
                .WithUnknownOdsCode();
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSCreateSessionOdsCodeNotSupportedOrNoNhsNumberErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenPatientHasUnknownSupplierAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P9Patient()
                .WithUnknownSupplierOdsCode();
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidCreateSessionOdsCodeNotSupportedOrNoNhsNumberErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenPatientHasUnknownSupplierIos(IIOSDriverWrapper driver)
        {
            var patient = new P9Patient()
                .WithUnknownSupplierOdsCode();
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSCreateSessionOdsCodeNotSupportedOrNoNhsNumberErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenPatientHasNoNhsNumberAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithNhsNumber(NhsNumber.None);
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidCreateSessionOdsCodeNotSupportedOrNoNhsNumberErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenPatientHasNoNhsNumberIos(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithNhsNumber(NhsNumber.None);
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSCreateSessionOdsCodeNotSupportedOrNoNhsNumberErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }
    }
}