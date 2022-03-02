using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.PreHome
{
    public sealed class TermsAndConditionsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal TermsAndConditionsPageContent(IWebInteractor webInteractor) => _interactor = webInteractor;

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Accept conditions of use");

        private WebText LogIntoAccountText => WebText.WithTagAndText(_interactor, "p", "You can log in to your NHS account using the NHS App or NHS website.");

        private WebText DisclaimerText => WebText.WithTagAndText(_interactor, "p", "To use your NHS account you must agree to our terms of use, privacy policy and cookies policy. You should read these carefully before logging in.");

        private WebLink DisclaimerTermsOfUseLink => DisclaimerText.WithChildLink("terms of use");

        private WebLink DisclaimerPrivacyPolicyLink => DisclaimerText.WithChildLink("privacy policy");

        private WebLink DisclaimerCookiesPolicyLink => DisclaimerText.WithChildLink("cookies policy");

        private WebText DoNotAgreeText => WebText.WithTagAndText(_interactor, "p", "If you do not agree, you will not be able to access or use your NHS account.");

        private WebText KeyPointsText => WebText.WithTagAndText(_interactor, "p", "Key points:");

        private WebText TermsAndConditionsNhsAppPurposePoint => WebText.WithTagAndText(
            _interactor,
            "li",
            "access to your NHS account is intended to provide you with information and services to help you " +
            "manage certain medical conditions or treatments - it is not a substitute for " +
            "seeking medical advice from a GP or other healthcare professional. Always follow " +
            "any medical advice given by your GP or other healthcare professional");

        private WebText TermsAndConditionsThirdPartyPoint => WebText.WithTagAndText(
            _interactor,
            "li",
            "information available through your NHS account comes from third parties, so we cannot " +
            "be responsible for its content or relevance to you. In particular, the settings used " +
            "by your GP surgery may affect what medical record information you can access");

        private WebText TermsAndConditionsTermsAndConditionsPoint => WebText.WithTagAndText(
            _interactor,
            "li",
            "logging in to your NHS account gives you access to a range of NHS services that may have their own terms " +
            "and privacy policies. You should read these so that you understand your rights " +
            "and how your data is used");

        private WebText CookieExplanationText => WebText.WithTagAndText(
            _interactor,
            "p",
            "We put small files (known as cookies) on your device. These " +
            "are used to make your NHS account work and improve your experience. You can manage " +
            "your cookies to opt out of using some of them.");

        private WebCheckbox AcceptTermsAndConditionsCheckbox => WebCheckbox.WithLabel(
            _interactor,
            "I understand and accept the terms of use and privacy policy. " +
            "I accept the use of 'strictly necessary' cookies as detailed in the cookies policy.");

        private WebText AcceptTermsAndConditionsText =>  WebText.WithTagAndText(
            _interactor,
            "span",
            "I understand and accept the terms of use and privacy policy. " +
            "I accept the use of 'strictly necessary' cookies as detailed in the cookies policy.");

        private WebLink TermsAndConditionsTermsOfUseLink => AcceptTermsAndConditionsText.WithChildLink("terms of use");

        private WebLink TermsAndConditionsPrivacyPolicyLink => AcceptTermsAndConditionsText.WithChildLink("privacy policy");

        private WebLink TermsAndConditionsCookiesPolicyLink => AcceptTermsAndConditionsText.WithChildLink("cookies policy");

        private WebText AcceptAnalyticCookiesText => WebText.WithTagAndText(
            _interactor,
            "label",
            "I accept the use of optional analytic cookies used to improve performance.");

        public WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");

        internal void AssertOnPage() => TitleText.AssertVisible();

        internal void AssertPageContent()
        {
            LogIntoAccountText.AssertVisible();
            DisclaimerText.AssertVisible();
            DisclaimerTermsOfUseLink.AssertVisible();
            DisclaimerPrivacyPolicyLink.AssertVisible();
            DisclaimerCookiesPolicyLink.AssertVisible();
            DoNotAgreeText.AssertVisible();
            KeyPointsText.AssertVisible();
            TermsAndConditionsNhsAppPurposePoint.AssertVisible();
            TermsAndConditionsThirdPartyPoint.AssertVisible();
            TermsAndConditionsTermsAndConditionsPoint.AssertVisible();
            CookieExplanationText.AssertVisible();
            TermsAndConditionsTermsOfUseLink.AssertVisible();
            TermsAndConditionsPrivacyPolicyLink.AssertVisible();
            TermsAndConditionsCookiesPolicyLink.AssertVisible();
            AcceptAnalyticCookiesText.AssertVisible();
        }

        public void AcceptTermsAndConditions()
        {
            AcceptAnalyticCookiesText.ScrollTo();
            AcceptTermsAndConditionsCheckbox.Click();
            ContinueButton.Click();
        }
    }
}
