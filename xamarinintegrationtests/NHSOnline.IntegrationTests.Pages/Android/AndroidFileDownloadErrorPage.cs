using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public sealed class AndroidFileDownloadErrorPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidFileDownloadErrorPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            _driver = driver;
        }

        private AndroidFullNavigation Navigation { get; }

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Sorry, something went wrong");

        private AndroidLabel WeCouldNotDownloadLabel => AndroidLabel
            .WithText(_driver, "We could not download this file, please try again.")
            .ScrollIntoView();

        private AndroidLabel IfYourProblemContinuesLabel => AndroidLabel
            .WithText(_driver, "If the problem continues and you need to download a document from your GP health record, contact your GP surgery.")
            .ScrollIntoView();

        private AndroidLabel TryAgainNowLabel => AndroidLabel
            .WithText(_driver, "If you’re trying to download another type of document, you can try again now. If the problem continues you may need to find another way to download the document.")
            .ScrollIntoView();

        private AndroidLink TryAgainButton => AndroidLink
            .WithContentDescription(_driver, "Try again")
            .ScrollIntoView();

        public static AndroidFileDownloadErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidFileDownloadErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public AndroidFileDownloadErrorPage AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
            WeCouldNotDownloadLabel.AssertVisible();
            IfYourProblemContinuesLabel.AssertVisible();
            TryAgainNowLabel.AssertVisible();
            TryAgainButton.AssertVisible();

            return this;
        }

        public void TryAgain() => TryAgainButton.Touch();

        public void Help() => Navigation.NavigateToHelp();

        public void Appointments() => Navigation.NavigateToAppointments();
    }
}
