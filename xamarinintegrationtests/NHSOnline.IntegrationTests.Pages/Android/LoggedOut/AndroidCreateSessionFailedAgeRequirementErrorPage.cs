using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidCreateSessionFailedAgeRequirementErrorPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidCreateSessionFailedAgeRequirementErrorPage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Cannot log in due to age restriction");

        private AndroidLabel MustBeOver13Text => AndroidLabel.WithText(_driver, "You must be aged 13 or over to use the NHS App due to legal restrictions.");
        private AndroidLabel IfAged12Text => AndroidLabel.WithText(_driver, "If you're aged 12, you can still get an NHS COVID Pass for travel.");

        private AndroidLink DigitalCovidPassLink => AndroidLink.WithContentDescription(_driver, "Get your digital NHS COVID Pass");
        private AndroidLink PaperCovidPassLink => AndroidLink.WithContentDescription(_driver, "Get your NHS COVID Pass letter sent to you by post");

        private AndroidLabel ContactGpText => AndroidLabel.WithText(_driver, "Contact your GP surgery to find out if you can access any GP services online.");
        private AndroidLabel ForUrgentMedicalAdvice => AndroidLabel.WithText(_driver, "For urgent medical advice, go to NHS 111 online or call 111.");

        private AndroidLink GoTo111Link => AndroidLink.WithContentDescription(_driver, "Go to 111.nhs.uk");

        public static AndroidCreateSessionFailedAgeRequirementErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidCreateSessionFailedAgeRequirementErrorPage(driver);
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
