using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb
{
    public class AskYourGpForAdviceStartPageContent
    {
        private readonly IWebInteractor _interactor;

        internal AskYourGpForAdviceStartPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Ask your GP for advice");

        private WebText ThisServiceIsProvidedByText => WebText.WithTagAndText(
            _interactor,
            "p",
            "This service is provided by an online consultation service provider, eConsult Health Ltd, " +
            "on behalf of your GP surgery. Find out more about online consultation services.");

        private WebLink FindOutMoreText => ThisServiceIsProvidedByText.WithChildLink(
            "Find out more about online consultation services.");

        private WebText ItTakesFiveText => WebText.WithTagAndText(
            _interactor,
            "p",
            "It takes around 5 minutes to answer a few questions about your condition.");

        private WebText ToSaveYouTypingText => WebText.WithTagAndText(
            _interactor,
            "p",
            "To save you typing in personal information the online consultation service needs, " +
            "you can use the personal information we already hold.");

        private WebCheckbox AllowDemographicDataCheckbox => WebCheckbox.WithLabel(
            _interactor,
            "Use my name, date of birth, NHS number and postal address with the online " +
            "consultation service as described in the NHS App privacy policy.");

        private WebLink PrivacyPolicyLink => AllowDemographicDataCheckbox.WithChildLink("NHS App privacy policy.");

        private WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");

        public void AssertOnPage() => TitleText.AssertVisible();

        public AskYourGpForAdviceStartPageContent AssertPageContent()
        {
            ThisServiceIsProvidedByText.AssertVisible();
            FindOutMoreText.AssertVisible();
            ItTakesFiveText.AssertVisible();
            ToSaveYouTypingText.AssertVisible();
            PrivacyPolicyLink.AssertVisible();
            return this;
        }

        public AskYourGpForAdviceStartPageContent  ClickDemographicsCheckbox()
        {
            AllowDemographicDataCheckbox.Click();
            return this;
        }

        public void Continue() => ContinueButton.Click();

        public void SelectFindOutMore() => FindOutMoreText.Click();

        public void SelectPrivacyPolicy() => PrivacyPolicyLink.Click();

    }
}