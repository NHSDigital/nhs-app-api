using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.PreHome;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Home
{
    public class IOS11TermsAndConditionsPage
    {
        private readonly IIOSInteractor _interactor;
        private TermsAndConditionsPageContent PageContent { get; }

        private IOS11CheckBox AcceptTermsAndConditionsCheckbox => IOS11CheckBox.StartsWith(_interactor, "I understand and accept the");

        private IOS11TermsAndConditionsPage(IIOSDriverWrapper driver)
        {
            _interactor = driver;
            PageContent = new TermsAndConditionsPageContent(driver.Web.NhsAppPreHomeWebView());
        }

        public static IOS11TermsAndConditionsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOS11TermsAndConditionsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AcceptTermsAndConditions()
        {
            PageContent.ContinueButton.ScrollTo();
            AcceptTermsAndConditionsCheckbox.Click();
            PageContent.ContinueButton.Click();
        }
    }
}