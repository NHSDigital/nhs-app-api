using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb
{
    public class AdditionalGpServicesStartPageContent
    {
        private readonly IWebInteractor _interactor;

        internal AdditionalGpServicesStartPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Additional GP services");

        private WebCheckbox UseMyDetailsCheckbox => WebCheckbox.WithLabel(
            _interactor,
            "Use my name, date of birth, NHS number and postal address with the " +
            "online consultation service as described in the NHS App privacy policy.");

        private WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public AdditionalGpServicesStartPageContent AcceptTermsAndConditions()
        {
            UseMyDetailsCheckbox.Click();
            return this;
        }

        public void Continue() => ContinueButton.Click();
    }
}