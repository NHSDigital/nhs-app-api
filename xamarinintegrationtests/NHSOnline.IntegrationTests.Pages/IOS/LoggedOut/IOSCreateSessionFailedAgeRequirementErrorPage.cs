using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSCreateSessionFailedAgeRequirementErrorPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSCreateSessionFailedAgeRequirementErrorPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => IOSLabel.WithText(_driver, "Cannot log in due to age restriction");

        private IOSLabel MustBeOver13Text => IOSLabel.WithText(_driver, "You must be aged 13 or over to use the NHS App due to legal restrictions.");
        private IOSLabel IfAged12Text => IOSLabel.WithText(_driver, "If you're aged 12, you can still get an NHS COVID Pass for travel.");

        private IOSLink DigitalCovidPassLink => IOSLink.WithText(_driver, "Get your digital NHS COVID Pass");
        private IOSLink PaperCovidPassLink => IOSLink.WithText(_driver, "Get your NHS COVID Pass letter sent to you by post");

        private IOSLabel ContactGpText => IOSLabel.WithText(_driver, "Contact your GP surgery to find out if you can access any GP services online.");
        private IOSLabel ForUrgentMedicalAdvice => IOSLabel.WithText(_driver, "For urgent medical advice, go to NHS 111 online or call 111.");

        private IOSLink GoTo111Link => IOSLink.WithText(_driver, "Go to 111.nhs.uk");

        public static IOSCreateSessionFailedAgeRequirementErrorPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSCreateSessionFailedAgeRequirementErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public void AssertPageElements()
        {
            MustBeOver13Text.AssertVisible();
            IfAged12Text.AssertVisible();
            DigitalCovidPassLink.AssertVisible();
            PaperCovidPassLink.AssertVisible();
            ContactGpText.AssertVisible();
            ForUrgentMedicalAdvice.AssertVisible();
            GoTo111Link.AssertVisible();
        }
    }
}
