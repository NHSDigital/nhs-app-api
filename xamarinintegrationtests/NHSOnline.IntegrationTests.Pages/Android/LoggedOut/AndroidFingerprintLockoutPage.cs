using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidFingerprintLockoutPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidFingerprintLockoutPage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Turn on fingerprint, face or iris again");

        private AndroidLabel CannotLogInTitle => AndroidLabel.WithText(_driver, "Cannot log in");

        public static AndroidFingerprintLockoutPage AssertOnPage(IAndroidDriverWrapper driver,
            bool screenshot = false)
        {
            var page = new AndroidFingerprintLockoutPage(driver);

            KnownIssue.BrowserStackUnableToVerifyAppVersionFailure()
                .ShouldExpect(() =>
                {
                    page.Title.AssertVisible();

                    if (screenshot)
                    {
                        driver.Screenshot(nameof(AndroidFingerprintLockoutPage));
                    }
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    page.CannotLogInTitle.AssertVisible();
                });

            return page;
        }

        public AndroidFingerprintLockoutPage AssertPageElements()
        {
            Title.AssertVisible();

            return this;
        }

    }
}
