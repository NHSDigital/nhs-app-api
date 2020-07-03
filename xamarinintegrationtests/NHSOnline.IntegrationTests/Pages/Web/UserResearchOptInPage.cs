using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Web
{
    internal sealed class UserResearchOptInPage
    {
        private readonly IWebInteractor _interactor;

        private UserResearchOptInPage(IWebInteractor interactor) => _interactor = interactor;

        private WebText Title => new WebText(_interactor, "h1", "Help improve the NHS App");

        private WebRadioOption OptIn => new WebRadioOption(
            _interactor,
            "Can we contact you to take part in NHS App user research?",
            "Yes, you can contact me about taking part in user research");

        private WebButton ContinueButton => new WebButton(_interactor, "Continue");

        internal static UserResearchOptInPage AssertOnPage(IWebInteractor interactor)
        {
            var page = new UserResearchOptInPage(interactor);
            page.Title.AssertVisible();
            return page;
        }

        internal UserResearchOptInPage OptInToUserResearch()
        {
            OptIn.Click();
            ContinueButton.Click();
            return this;
        }
    }
}