using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings.LegalAndCookies.ManageCookies
{
    public class ManageCookiesPageContent
    {
        private readonly IWebInteractor _interactor;

        internal ManageCookiesPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Manage cookies");

        private WebToggle AllowOptionalCookiesToggle
            => WebToggle.WithLabel(_interactor, "Allow optional analytic cookiesI accept the use of optional analytic " +
                                                "cookies used to improve the performance of the NHS App");

        internal void AssertOnPage()
        {
            TitleText.ScrollTo();
            TitleText.AssertVisible();
        }

        public void ToggleOptionalCookies() => AllowOptionalCookiesToggle.ToggleOn();

        public void ToggleOptionalCookiesOn()
        {
            AllowOptionalCookiesToggle.ToggleOn();
            using var timeout = ExtendedTimeout.FromSeconds(10);
            AllowOptionalCookiesToggle.AssertToggledOn();
        }
    }
}