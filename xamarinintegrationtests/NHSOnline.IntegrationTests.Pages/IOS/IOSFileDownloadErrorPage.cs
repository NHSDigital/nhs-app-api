using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public sealed class IOSFileDownloadErrorPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSFileDownloadErrorPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            _driver = driver;
        }

        private IOSFullNavigation Navigation { get; }


        private IOSLabel Title => IOSLabel.WithText(_driver, "Cannot download file");

        private IOSLabel TryAgainLabel => IOSLabel
            .WithText(_driver, "Try again. If you still cannot download this file and it’s a document from your GP health record, contact your GP surgery.")
            .ScrollIntoView();

        private IOSLabel OtherTypesOfDocumentLabel => IOSLabel
            .WithText(_driver, "For all other types of documents, you’ll need to find another way to view them.")
            .ScrollIntoView();

        private IOSLink GetDocumentDownloadHelpLink => IOSLink
            .WithText(_driver, "Get help with downloading documents from your health record");

        private IOSLink TryAgainButton => IOSLink
            .WithText(_driver, "Try again");

        public static IOSFileDownloadErrorPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSFileDownloadErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public IOSFileDownloadErrorPage AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            TryAgainLabel.AssertVisible();
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
