using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class AdvicePageContent
    {
        private readonly IWebInteractor _interactor;

        internal AdvicePageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Advice");

        private WebMenuItem GetAdviceMenuItem => WebMenuItem.WithTitle(_interactor, "Get advice about coronavirus");

        private WebMenuItem ShareConditionsMenuItem => WebMenuItem.WithTitle(_interactor, "Share conditions and treatments");

        private WebMenuItem UseNhsOnlineMenuItem => WebMenuItem.WithTitle(_interactor, "Use NHS 111 online");

        private WebMenuItem AskGpMenuItem => WebMenuItem.WithTitle(_interactor, "Ask your GP for advice");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            GetAdviceMenuItem,
            ShareConditionsMenuItem,
            UseNhsOnlineMenuItem,
            AskGpMenuItem
        };

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public AdvicePageContent AssertPageElements()
        {
            Title.AssertVisible();
            return this;
        }
    }
}
