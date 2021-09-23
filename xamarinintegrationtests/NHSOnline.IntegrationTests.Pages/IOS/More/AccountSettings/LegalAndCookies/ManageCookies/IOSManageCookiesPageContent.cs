using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings.LegalAndCookies.ManageCookies
{
    public class IOSManageCookiesPageContent
    {
        private readonly IIOSInteractor _interactor;

        public IOSManageCookiesPageContent(IIOSInteractor interactor)
        {
            _interactor = interactor;
        }

        private IOSLabel OptionalCookiesToggle => IOSLabel.WithText(_interactor, "Allow optional analytic cookies");

        public void ToggleOptionalCookies() => OptionalCookiesToggle.Click();
    }
}