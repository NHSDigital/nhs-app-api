using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class UpdatedTermsAndConditionsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal UpdatedTermsAndConditionsPageContent(IWebInteractor webInteractor) => _interactor = webInteractor;

        private WebText Title => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Updated conditions of use");

        private WebText UpdatedConditionsOfUseText => WebText.WithTagAndText(_interactor, "p",
            "We've made some important changes to our conditions of use. " +
            "To continue using the NHS App, you need to agree to our updated terms of use, " +
            "privacy policy and cookies policy.");

        private WebLink UpdateTermsOfUseLink =>
            UpdatedConditionsOfUseText.WithChildLink("terms of use");

        private WebLink UpdatePrivacyPolicyLink =>
            UpdatedConditionsOfUseText.WithChildLink("privacy policy");

        private WebLink UpdateCookiesPolicyLink =>
            UpdatedConditionsOfUseText.WithChildLink("cookies policy");


        private WebText DoNotAgreeText => WebText.WithTagAndText(
            _interactor,
            "p",
            "If you don't agree, you won't be able to continue accessing or using the NHS App.");

        private WebCheckbox AcceptTermsAndConditionsCheckbox => WebCheckbox.WithLabel(
            _interactor,
            "I understand and agree to the updated terms of use and privacy policy. " +
            "I agree to the use of 'strictly necessary' cookies as described in the updated cookies policy.");

        private WebText AcceptTermsAndConditionsText => WebText.WithTagAndText(
            _interactor,
            "span",
            "I understand and agree to the updated terms of use and privacy policy. " +
            "I agree to the use of 'strictly necessary' cookies as described in the updated cookies policy.");

        private WebLink AcceptTermsOfUseLink => AcceptTermsAndConditionsText.WithChildLink("terms of use");

        private WebLink AcceptPrivacyPolicyLink => AcceptTermsAndConditionsText.WithChildLink("privacy policy");

        private WebLink AcceptCookiesPolicyLink => AcceptTermsAndConditionsText.WithChildLink("cookies policy");

        private WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        internal void AssertPageContent()
        {
            UpdatedConditionsOfUseText.AssertVisible();
            UpdateTermsOfUseLink.AssertVisible();
            UpdatePrivacyPolicyLink.AssertVisible();
            UpdateCookiesPolicyLink.AssertVisible();
            DoNotAgreeText.AssertVisible();
            AcceptTermsAndConditionsText.AssertVisible();
            AcceptTermsOfUseLink.AssertVisible();
            AcceptPrivacyPolicyLink.AssertVisible();
            AcceptCookiesPolicyLink.AssertVisible();
        }

        public void AcceptTermsAndConditions()
        {
            AcceptTermsAndConditionsCheckbox.Click();
            ContinueButton.Click();
        }
    }
}