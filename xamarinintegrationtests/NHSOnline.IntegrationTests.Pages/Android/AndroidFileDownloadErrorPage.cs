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

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Cannot download file");

        private AndroidLabel TryAgainNowLabel => AndroidLabel
            .WithText(_driver, "Try again. If you still cannot download this file and it’s a document from your GP health record, contact your GP surgery.")
            .ScrollIntoView();

        private AndroidLabel OtherTypesOfDocumentLabel => AndroidLabel
            .WithText(_driver, "For all other types of documents, you’ll need to find another way to view them.")
            .ScrollIntoView();

        private AndroidLink GetDocumentDownloadHelpLink => AndroidLink
            .WithContentDescription(_driver, "Get help with downloading documents from your health record")
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
            Navigation.AssertNavigationIconsArePresent(); ;
            TryAgainNowLabel.AssertVisible();
            OtherTypesOfDocumentLabel.AssertVisible();
            GetDocumentDownloadHelpLink.AssertVisible();
            TryAgainButton.AssertVisible();

            return this;
        }

        public void TryAgain() => TryAgainButton.Touch();

        public void GetDocumentDownloadHelp() => GetDocumentDownloadHelpLink.Touch();

        public void Help() => Navigation.NavigateToHelp();

        public void Appointments() => Navigation.NavigateToAppointments();
    }
}
