using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public sealed class TermsAndConditionsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal TermsAndConditionsPageContent(IWebInteractor webInteractor) => _interactor = webInteractor;

        private WebText Title => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Accept conditions of use");

        private WebText DisclaimerParagraph => WebText.WithTagAndText(
            _interactor,
            "p",
            "To use the NHS App you must agree to our terms of use, privacy policy and " +
            "cookies policy. You should read these carefully before using the app.");

        private WebLink DisclaimerTermsOfUseLink => DisclaimerParagraph.WithChildLink("terms of use");

        private WebLink DisclaimerPrivacyPolicyLink => DisclaimerParagraph.WithChildLink("privacy policy");

        private WebLink DisclaimerCookiesPolicyLink => DisclaimerParagraph.WithChildLink("cookies policy");

        private WebText DoNotAgreeText => WebText.WithTagAndText(
            _interactor,
            "p",
            "If you do not agree, you won't be able to access or use the NHS App.");

        private WebText KeyPointsText => WebText.WithTagAndText(
            _interactor,
            "p",
            "Key points:");

        private WebText TermsAndConditionsNhsAppPurposePoint => WebText.WithTagAndText(
            _interactor,
            "li",
            "The NHS App is intended to provide you with information and services to help you " +
            "manage certain medical conditions or treatments - it is not a substitute for " +
            "seeking medical advice from a GP or other healthcare professional. Always follow " +
            "any medical advice given by your GP or other healthcare professional");

        private WebText TermsAndConditionsThirdPartyPoint => WebText.WithTagAndText(
            _interactor,
            "li",
            "Information available through the NHS App comes from third parties, so we cannot " +
            "be responsible for its content or relevance to you. In particular, the settings used " +
            "by your GP surgery may affect what medical record information you can access");

        private WebText TermsAndConditionsTermsAndConditionsPoint => WebText.WithTagAndText(
            _interactor,
            "li",
            "The NHS App gives you access to a range of NHS services that may have their own terms " +
            "and privacy policies. You should read these so that you understand your rights " +
            "and how your data is used");

        private WebText CookieExplanationText => WebText.WithTagAndText(
            _interactor,
            "p",
            "The NHS App puts small files (known as cookies) on your device. These " +
            "are used to make the app work and improve your experience. You can manage " +
            "your cookies to opt out of using some of them.");

        private WebLink ManageCookiesLink => CookieExplanationText.WithChildLink("manage your cookies");

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
            "I accept the use of optional analytic cookies used to improve the performance of the NHS App.");

        private WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        internal void AssertPageContent()
        {
            DisclaimerParagraph.AssertVisible();
            DisclaimerTermsOfUseLink.AssertVisible();
            DisclaimerPrivacyPolicyLink.AssertVisible();
            DisclaimerCookiesPolicyLink.AssertVisible();
            DoNotAgreeText.AssertVisible();
            KeyPointsText.AssertVisible();
            TermsAndConditionsNhsAppPurposePoint.AssertVisible();
            TermsAndConditionsThirdPartyPoint.AssertVisible();
            TermsAndConditionsTermsAndConditionsPoint.AssertVisible();
            CookieExplanationText.AssertVisible();
            ManageCookiesLink.AssertVisible();
            TermsAndConditionsTermsOfUseLink.AssertVisible();
            TermsAndConditionsPrivacyPolicyLink.AssertVisible();
            TermsAndConditionsCookiesPolicyLink.AssertVisible();
            AcceptAnalyticCookiesText.AssertVisible();
        }

        public void AcceptTermsAndConditions()
        {
            AcceptTermsAndConditionsCheckbox.Click();
            ContinueButton.Click();
        }
    }
}
