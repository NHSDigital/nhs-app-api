using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public class AndroidAcceptNhsTermsOfUsePageContent
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidAcceptNhsTermsOfUsePageContent(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => AndroidLabel.WithText(
            _driver,
            "You need to accept NHS login terms of use to continue");

        private AndroidLabel YouCantUseTheNhsApp => AndroidLabel.WithText(
            _driver,
            "You cannot use the NHS App if you have not accepted NHS login terms of use.");

        private AndroidLabel ContactGpSurgery => AndroidLabel.WithText(
            _driver,
            "If you need to book an appointment or get a prescription now, contact your GP surgery directly.");

        private AndroidLabel UrgentMedicalAdvice => AndroidLabel.WithText(
            _driver,
            "For urgent medical advice, visit 111.nhs.uk or call 111.");

        public static AndroidAcceptNhsTermsOfUsePageContent AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidAcceptNhsTermsOfUsePageContent(driver);
            page.Title.AssertVisible();
            return page;
        }

        public AndroidAcceptNhsTermsOfUsePageContent AssertPageContent()
        {
            Title.AssertVisible();
            YouCantUseTheNhsApp.AssertVisible();
            ContactGpSurgery.AssertVisible();
            UrgentMedicalAdvice.AssertVisible();
            return this;
        }
    }
}