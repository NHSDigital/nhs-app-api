using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public sealed class UserResearchOptInPageContent
    {
        private readonly IWebInteractor _interactor;

        internal UserResearchOptInPageContent(IWebInteractor webInteractor) => _interactor = webInteractor;

        private WebText Title => new WebText(_interactor, "h1", "Help improve the NHS App");

        private WebRadioOption OptIn => new WebRadioOption(
            _interactor,
            "Can we contact you to take part in NHS App user research?",
            "Yes, you can contact me about taking part in user research");

        private WebButton ContinueButton => new WebButton(_interactor, "Continue");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public void OptInToUserResearch()
        {
            OptIn.Click();
            ContinueButton.Click();
        }
    }
}