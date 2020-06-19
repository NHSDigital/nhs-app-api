using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Web
{
    internal sealed class DesktopLoggedOutHomePage
    {
        private readonly IWebInteractor _interactor;

        private DesktopLoggedOutHomePage(IWebInteractor interactor) => _interactor = interactor;

        private WebText Title => new WebText(_interactor, "h1", "Access your NHS services");

        private WebButton ContinueWithNhsLoginButton => new WebButton(_interactor, "Continue with NHS login");

        internal static DesktopLoggedOutHomePage AssertOnPage(IWebInteractor interactor)
        {
            var page = new DesktopLoggedOutHomePage(interactor);
            page.Title.AssertVisible();
            return page;
        }

        internal DesktopLoggedOutHomePage ContinueWithNhsLogin()
        {
            ContinueWithNhsLoginButton.Click();
            return this;
        }
    }
}