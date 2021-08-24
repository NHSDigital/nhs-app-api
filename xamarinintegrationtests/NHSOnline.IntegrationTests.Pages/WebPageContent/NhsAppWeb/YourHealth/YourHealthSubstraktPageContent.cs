using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth
{
    public class YourHealthSubstraktPageContent : YourHealthPageContent
    {
        private readonly IWebInteractor _interactor;

        internal YourHealthSubstraktPageContent(IWebInteractor interactor) : base(interactor) => _interactor = interactor;

        private WebMenuItem SubstraktUpdateYourPersonalDetailsMenuItem => WebMenuItem.WithTitle(_interactor, "Update your personal details");

        private WebText SubstraktUpdateYourPersonalDetailsText => WebText.WithTagAndText(_interactor, "p", "Fill out a form to let your GP surgery know which details have changed");

        public void AssertElements()
        {
            SubstraktUpdateYourPersonalDetailsMenuItem.AssertVisible();
            SubstraktUpdateYourPersonalDetailsText.AssertVisible();
        }

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            CovidPassMenuItem,
            VaccineRecordMenuItem,
            GpHeathRecordMenuItem,
            SubstraktUpdateYourPersonalDetailsMenuItem,
            OrganDonationMenuItem,
            NdopMenuItem
        };

        public void NavigateToUpdateYourPersonalDetails() => SubstraktUpdateYourPersonalDetailsMenuItem.Click();

        public void KeyboardNavigateToSubstrakt(AndroidKeyboardNavigation navigation) =>
            KeyboardNavigateToAndActivateMenuItem(SubstraktUpdateYourPersonalDetailsMenuItem, navigation);
    }
}