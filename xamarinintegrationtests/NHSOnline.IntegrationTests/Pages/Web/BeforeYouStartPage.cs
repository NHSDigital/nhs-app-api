using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Web
{
    internal sealed class BeforeYouStartPage
    {
        private readonly IWebInteractor _interactor;

        private BeforeYouStartPage(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => new WebText(_interactor, "h1", "Before you start");

        private WebButton ContinueButton => new WebButton(_interactor, "Continue");

        internal static BeforeYouStartPage AssertOnPage(IWebInteractor interactor)
        {
            var page = new BeforeYouStartPage(interactor);
            page.TitleText.AssertVisible();
            return page;
        }

        internal BeforeYouStartPage Continue()
        {
            ContinueButton.Click();
            return this;
        }
    }
}