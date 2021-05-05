using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
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

        private WebMenuItem GetAdviceMenuItem => WebMenuItem.WithTitle(_interactor, "Get advice about coronavirus (COVID-19)");

        private WebMenuItem SearchConditionsMenuItem => WebMenuItem.WithTitle(_interactor, "Search conditions and treatments");

        private WebMenuItem UseNhsOnlineMenuItem => WebMenuItem.WithTitle(_interactor, "Use NHS 111 online");

        private WebMenuItem AskGpMenuItem => WebMenuItem.WithTitle(_interactor, "Ask your GP for advice");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            GetAdviceMenuItem,
            SearchConditionsMenuItem,
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

        public void OneOneOne()
        {
            UseNhsOnlineMenuItem.Click();
        }

        public void AToZ()
        {
            SearchConditionsMenuItem.Click();
        }

        public void KeyboardNavigateToOneOneOne(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(UseNhsOnlineMenuItem, navigation);

        public void KeyboardNavigateToAToZ(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(SearchConditionsMenuItem, navigation);

        private void KeyboardNavigateToAndActivateMenuItem(IFocusable menuItem, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(GetAdviceMenuItem, menuItem);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}
