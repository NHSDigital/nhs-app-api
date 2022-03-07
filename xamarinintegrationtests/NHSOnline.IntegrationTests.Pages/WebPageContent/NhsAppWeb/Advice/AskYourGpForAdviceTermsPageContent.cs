using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Advice
{
    public class AskYourGpForAdviceTermsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal AskYourGpForAdviceTermsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Ask your GP for advice");

        private WebText PolicyText => WebText.WithTagAndText(
            _interactor,
            "p",
            "To start, please agree to the privacy notice applicable to online consultation services.");

        public AskYourGpForAdviceTermsPageContent AssertOnPage()
        {
            TitleText.AssertVisible();
            PolicyText.AssertVisible();
            return this;
        }

    }
}