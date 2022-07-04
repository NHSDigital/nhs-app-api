using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidCreateSessionInternalServerErrorPage
    {
        private readonly IAndroidDriverWrapper _driver;
        private AndroidSlimCloseNavigation Navigation { get; }

        private AndroidCreateSessionInternalServerErrorPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidSlimCloseNavigation(driver);
            _driver = driver;
        }

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "The service is unavailable");
        private AndroidLabel ThisMightBeATemporaryProblemText => AndroidLabel.WithText(_driver, "This might be a temporary problem.");
        private AndroidLink GoBackAndTryLoggingInAgainLink => AndroidLink.WhichMatches(_driver, "Go back and try logging in again");
        private AndroidLabel IfYouStillCannotLoginText => AndroidLabel.WithText(_driver, "If you still cannot log in, try again later.");
        private AndroidLabel OtherServicesYouCanUseText => AndroidLabel.WithText(_driver, "Other services you can use");
        private AndroidLink GetYourNhsCovidPassOnlineLink => AndroidLink.WhichMatches(_driver, "Get your NHS COVID Pass online");
        private AndroidLabel GetMedicalAdviceText => AndroidLabel.WithText(_driver, "Get medical advice");
        private AndroidLabel IfYouNeedText => AndroidLabel.WithText(_driver, "If you need to book an appointment or get a prescription now, use your GP surgery's website or call the surgery directly.");
        private AndroidLink ForUrgentMedicalAdviceLink => AndroidLink.WhichMatches(_driver, "For urgent medical advice, go to 111.nhs.uk or call 111");

        private IEnumerable<IFocusable> GetAllKeyboardNavigationFocusableElements()
        {
            var headerList = Navigation.KeyboardNavigation.GetFocusableElements();
            var pageFocusableList = new[] {GoBackAndTryLoggingInAgainLink, GetYourNhsCovidPassOnlineLink, ForUrgentMedicalAdviceLink};

            return headerList.Concat(pageFocusableList);
        }
        private AndroidKeyboardNavigation KeyboardPageContentNavigation =>
            AndroidKeyboardNavigation.WithExpectedFocusableElements(_driver, GetAllKeyboardNavigationFocusableElements());

        public static AndroidCreateSessionInternalServerErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidCreateSessionInternalServerErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public AndroidCreateSessionInternalServerErrorPage AssertPageElements()
        {
            ThisMightBeATemporaryProblemText.AssertVisible();
            GoBackAndTryLoggingInAgainLink.AssertVisible();
            IfYouStillCannotLoginText.AssertVisible();
            OtherServicesYouCanUseText.AssertVisible();
            GetYourNhsCovidPassOnlineLink.AssertVisible();
            GetMedicalAdviceText.AssertVisible();
            IfYouNeedText.AssertVisible();
            ForUrgentMedicalAdviceLink.AssertVisible();
            return this;
        }

        public void GetUrgentMedicalAdvice() => ForUrgentMedicalAdviceLink.Touch();
        public void GetYourNhsCovidPassOnline() => GetYourNhsCovidPassOnlineLink.Touch();
        public void BackToLogin() => GoBackAndTryLoggingInAgainLink.Touch();

        public void KeyboardNavigateToAndActivateGetUrgentMedicalAdvice() => KeyboardNavigateToAndActivateFocusable(ForUrgentMedicalAdviceLink);
        public void KeyboardNavigateToAndActivateGetYourNhsCovidPassOnline() => KeyboardNavigateToAndActivateFocusable(GetYourNhsCovidPassOnlineLink);
        public void KeyboardNavigateToAndActivateBackToLogin() => KeyboardNavigateToAndActivateFocusable(GoBackAndTryLoggingInAgainLink);

        private void KeyboardNavigateToAndActivateFocusable(IFocusable focusable)
        {
            KeyboardPageContentNavigation.TabTo(focusable);
            KeyboardPageContentNavigation.PressEnterKey();
        }
    }
}