using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public class AndroidForcedUpdatePage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidForcedUpdatePage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "You need to update the NHS App");

        private AndroidLabel YouWillNotBeAbleToUseItLabel => AndroidLabel.WithText(_driver, "You will not be able to use it until you do.");

        private AndroidLink UpdateLink => AndroidLink.WithText(_driver, "Update the NHS App now");

        public static AndroidForcedUpdatePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidForcedUpdatePage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public AndroidForcedUpdatePage AssertPageElements()
        {
            YouWillNotBeAbleToUseItLabel.AssertVisible();
            return this;
        }

        public void Upgrade() => UpdateLink.Touch();
    }
}