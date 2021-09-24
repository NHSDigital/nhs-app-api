using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSCreateSessionFailedAgeRequirementErrorPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSCreateSessionFailedAgeRequirementErrorPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => IOSLabel.WithText(_driver, "Login failed");

        private IOSLabel DueToLegalRestrictionsText => IOSLabel.WithText(_driver, "Due to legal restrictions, you cannot use the NHS App until you are at least 13 years old. You can still contact your GP surgery to access your NHS services.");
        private IOSLabel ForUrgentMedicalAdvice => IOSLabel.WithText(_driver, "For urgent medical advice, use NHS 111 online or call 111.");
        private IOSLink GoTo111Link => IOSLink.WithText(_driver, "Go to 111.nhs.uk");

        public static IOSCreateSessionFailedAgeRequirementErrorPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSCreateSessionFailedAgeRequirementErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public void AssertPageElements()
        {
            DueToLegalRestrictionsText.AssertVisible();
            ForUrgentMedicalAdvice.AssertVisible();
            GoTo111Link.AssertVisible();
        }
    }
}