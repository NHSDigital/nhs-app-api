using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public sealed class UserResearchOptInPageContent
    {
        private readonly IWebInteractor _interactor;

        internal UserResearchOptInPageContent(IWebInteractor webInteractor)
        {
            _interactor = webInteractor;

            WhatsInvolvedExpander =  WebLinkExpander.WithText(_interactor, "What's involved?");

            WhatsInvolved =  WhatsInvolvedExpander.Contains(interactor => WebText.WithTagAndText(
                interactor,
                "p",
                "We'll add you to our user research panel and email you " +
                "a short survey to fill in about you and your health. " +
                "Your answers will help make sure you get invited to user research that's relevant to you."));

            OnceYourSignedUp = WhatsInvolvedExpander.Contains(interactor => WebText.WithTagAndText(
                interactor,
                "p",
                "Once you're signed up, you might be asked to:"));

            TryOutNewFeatures = WhatsInvolvedExpander.Contains(interactor => WebText.WithTagAndText(
                interactor,
                "li",
                "try out new features"));

            AnswerQuestionsByEmail = WhatsInvolvedExpander.Contains(interactor => WebText.WithTagAndText(
                interactor,
                "li",
                "answer more questions by email"));

            TalkToResearchers = WhatsInvolvedExpander.Contains(interactor =>  WebText.WithTagAndText(
                interactor,
                "li",
                "talk to our researchers about your experience of using the app"));

            LeaveResearchPanel = WhatsInvolvedExpander.Contains(interactor => WebText.WithTagAndText(
                interactor,
                "p",
                "You can always say no to an invite and you can leave the user research panel at any time."));
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Help improve the NHS App");

        private WebText TakingPart => WebText.WithTagAndText(
            _interactor,
            "p",
            "We would like to contact you about taking part in user " +
            "research to improve the NHS App and connected services.");

        private WebLinkExpander WhatsInvolvedExpander { get; }

        private WebText WhatsInvolved { get; }

        private WebText OnceYourSignedUp { get; }

        private WebText TryOutNewFeatures { get; }

        private WebText AnswerQuestionsByEmail { get; }

        private WebText TalkToResearchers { get; }

        private WebText LeaveResearchPanel { get; }

        private WebText HowYourInformationWillBeUsed => WebText.WithTagAndText(
            _interactor,
            "p",
            "Your information will only be used to contact you about the NHS App user " +
            "research panel. It will not be shared with anyone else and you can unsubscribe " +
            "at any time. Read our privacy policy to find out how we use and protect your data.");

        private WebRadioOption OptIn => WebRadioOption.InFieldsetLegendWithLabel(
            _interactor,
            "Can we contact you to take part in NHS App user research?",
            "Yes, you can contact me about taking part in user research");

        private WebRadioOption OptOut => WebRadioOption.InFieldsetLegendWithLabel(
            _interactor,
            "Can we contact you to take part in NHS App user research?",
            "No, do not contact me");

        private WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");

        internal void AssertOnPage()
        {
            using var timeout = ExtendedTimeout.FromSeconds(5);

            Title.AssertVisible();
        }

        public void AssertPageContent()
        {
            TakingPart.AssertVisible();
            WhatsInvolvedExpander.AssertVisible();
            WhatsInvolvedExpander.AssertCollapsed();
            WhatsInvolvedExpander.Toggle();
            WhatsInvolvedExpander.AssertExpanded();
            HowYourInformationWillBeUsed.AssertVisible();
            OptIn.AssertVisible();
            OptOut.AssertVisible();
        }

        public void OptInToUserResearch()
        {
            ContinueButton.ScrollTo();
            OptIn.Click();
            ContinueButton.Click();
        }
    }
}