using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Web
{
    internal sealed class NativeLoggedOutHomePage
    {
        private readonly IWebInteractor _interactor;

        private NativeLoggedOutHomePage(IWebInteractor interactor) => _interactor = interactor;

        private WebText Title => new WebText(_interactor, "h2", "How are you feeling today?");

        private WebButton ContinueWithNhsLoginButton => new WebButton(_interactor, "Continue with NHS login");

        internal static NativeLoggedOutHomePage AssertOnPage(IWebInteractor interactor)
        {
            var page = new NativeLoggedOutHomePage(interactor);
            page.Title.AssertVisible();
            return page;
        }

        internal NativeLoggedOutHomePage ContinueWithNhsLogin()
        {
            ContinueWithNhsLoginButton.Click();
            return this;
        }
    }
}
