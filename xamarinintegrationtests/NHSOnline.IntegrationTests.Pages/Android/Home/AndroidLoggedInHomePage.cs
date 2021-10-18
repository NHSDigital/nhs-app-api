using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Home;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Home
{
    public class AndroidLoggedInHomePage
    {
        private HashSet<string> KeyboardNavigationOds =
            new()
            {
                EmisPatientOds.Pkb.ToOdsCodeString(),
                EmisPatientOds.AllSilversEnabled.ToOdsCodeString(),
                EmisPatientOds.MyCareView.ToOdsCodeString(),
                EmisPatientOds.SecondaryCareView.ToOdsCodeString(),
                EmisPatientOds.Substrakt.ToOdsCodeString(),
                EmisPatientOds.Cie.ToOdsCodeString(),
                EmisPatientOds.Gncr.ToOdsCodeString()
            };
        private string InvalidPatientAssertionMessage = "Emis PKB patient is required to use keyboard navigation";
        public AndroidFullNavigation Navigation { get; }
        public LoggedInHomePageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidLoggedInHomePage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new LoggedInHomePageContent(driver.Web.NhsAppLoggedInWebView(),
                LoggedInHomePageContent.BioFingerprint);
        }

        public static AndroidLoggedInHomePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidLoggedInHomePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidLoggedInHomePage AssertPageDisplayedFor(string name)
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertNameDisplayedFor(name);
            return this;
        }

        private AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation
            .WithExpectedFocusableElements(_driver, GetAllKeyboardHomeNavigationFocusableElements());

        private IEnumerable<IFocusable> GetAllKeyboardHomeNavigationFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();
            var pageFocusableList = PageContent.FocusableElements;

            return headerFocusableList.Concat(pageFocusableList).Concat(footerFocusableList);
        }

        public void ProveYourIdentityContinue() => PageContent.ProveYourIdentityContinue();

        public void NavigateToAppointments() => Navigation.NavigateToAppointments();

        public void KeyboardNavigateToAdvice(EmisPatient patient)
        {
            AssertValidKeyboardPatientUsed(patient);
            Navigation.KeyboardNavigateToAdvice(KeyboardPageContentNavigation);
        }

        public void KeyboardNavigateToAppointments(EmisPatient patient)
        {
            AssertValidKeyboardPatientUsed(patient);
            Navigation.KeyboardNavigateToAppointments(KeyboardPageContentNavigation);
        }

        public void KeyboardNavigatePrescriptions(EmisPatient patient)
        {
            AssertValidKeyboardPatientUsed(patient);
            Navigation.KeyboardNavigateToPrescriptions(KeyboardPageContentNavigation);
        }

        public void KeyboardNavigateToYourHealth(EmisPatient patient)
        {
            AssertValidKeyboardPatientUsed(patient);
            Navigation.KeyboardNavigateToYourHealth(KeyboardPageContentNavigation);
        }

        public void KeyboardNavigateToMessages(EmisPatient patient)
        {
            AssertValidKeyboardPatientUsed(patient);
            Navigation.KeyboardNavigateToMessages(KeyboardPageContentNavigation);
        }

        public void KeyboardNavigateToHelp(EmisPatient patient)
        {
            AssertValidKeyboardPatientUsed(patient);
            Navigation.KeyboardNavigateToHelp(KeyboardPageContentNavigation);
        }

        public void KeyboardNavigateToMore(EmisPatient patient)
        {
            AssertValidKeyboardPatientUsed(patient);
            Navigation.KeyboardNavigateToMore(KeyboardPageContentNavigation);
        }

        private void AssertValidKeyboardPatientUsed(EmisPatient patient)
        {
            Assert.IsTrue(KeyboardNavigationOds.Contains(patient.OdsCode), InvalidPatientAssertionMessage);
        }
    }
}