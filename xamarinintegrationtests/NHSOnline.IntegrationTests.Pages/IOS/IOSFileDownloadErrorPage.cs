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

        private IOSLabel Title => IOSLabel.WithText(_driver, "Sorry, something went wrong");

        private IOSLabel WeCouldNotDownloadLabel => IOSLabel
            .WithText(_driver, "We could not download this file, please try again.")
            .ScrollIntoView();

        private IOSLabel IfYourProblemContinuesLabel => IOSLabel
            .WithText(_driver, "If the problem continues and you need to download a document from your GP health record, contact your GP surgery.")
            .ScrollIntoView();

        private IOSLabel TryAgainNowLabel => IOSLabel
            .WithText(_driver, "If you’re trying to download another type of document, you can try again now. If the problem continues you may need to find another way to download the document.")
            .ScrollIntoView();

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
