using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidFullNavigation
    {
        private readonly IAndroidDriverWrapper _driver;

        internal AndroidFullNavigation(IAndroidDriverWrapper driver)
        {
            _driver = driver;
        }

        private AndroidIcon HomeIcon => AndroidIcon.WithName(_driver,"NHS App Home");

        private AndroidIcon HelpIcon => AndroidIcon.WithName(_driver,"Help");

        private AndroidIcon MoreIcon => AndroidIcon.WithName(_driver,"More");

        private AndroidIcon AdviceIcon => AndroidIcon.WithName(_driver,"Advice");

        private AndroidIcon AppointmentsIcon => AndroidIcon.WithName(_driver,"Appointments");

        private AndroidIcon PrescriptionsIcon => AndroidIcon.WithName(_driver,"Prescriptions");

        private AndroidIcon YourHealthIcon => AndroidIcon.WithName(_driver,"Your health");

        private AndroidIcon MessagesIcon => AndroidIcon.WithName(_driver,"Messages");

        internal AndroidKeyboardNavigation KeyboardHeaderNavigation => AndroidKeyboardNavigation.WithExpectedFocusableElements(
            _driver,
            HomeIcon,
            HelpIcon,
            MoreIcon);

        public AndroidKeyboardNavigation KeyboardFooterNavigation => AndroidKeyboardNavigation.WithExpectedFocusableElements(
            _driver,
            AdviceIcon,
            AppointmentsIcon,
            PrescriptionsIcon,
            YourHealthIcon,
            MessagesIcon);


        public void AssertNavigationIconsArePresent()
        {
            HomeIcon.AssertVisible();
            HelpIcon.AssertVisible();
            MoreIcon.AssertVisible();
            AdviceIcon.AssertVisible();
            AppointmentsIcon.AssertVisible();
            PrescriptionsIcon.AssertVisible();
            YourHealthIcon.AssertVisible();
            MessagesIcon.AssertVisible();
        }

        public void AssertHomeIconIsPresent()
        {
            HomeIcon.AssertVisible();
        }

        public void AssertNoIconsSelected()
        {
            AdviceIcon.AssertNotSelected();
            AppointmentsIcon.AssertNotSelected();
            YourHealthIcon.AssertNotSelected();
            MessagesIcon.AssertNotSelected();
            PrescriptionsIcon.AssertNotSelected();
        }

        public void NavigateToHome()
        {
            HomeIcon.Click();
        }

        public void NavigateToHelp()
        {
            HelpIcon.Click();
        }

        public void NavigateToMore()
        {
            MoreIcon.Click();
        }

        public void NavigateToAdvice()
        {
            AdviceIcon.Click();
        }

        public void NavigateToAppointments()
        {
            AppointmentsIcon.Click();
        }

        public void NavigateToPrescriptions()
        {
            PrescriptionsIcon.Click();
        }

        public void NavigateToYourHealth()
        {
            YourHealthIcon.Click();
        }

        public void NavigateToMessages()
        {
            MessagesIcon.Click();
        }

        public void AssertNoNavigationIconsSelected()
        {
            AdviceIcon.AssertNotSelected();
            AppointmentsIcon.AssertNotSelected();
            PrescriptionsIcon.AssertNotSelected();
            YourHealthIcon.AssertNotSelected();
            MessagesIcon.AssertNotSelected();
        }

        public void AssertAdviceSelected()
        {
            AdviceIcon.AssertSelected();
        }

        public void AssertAppointmentsSelected()
        {
            AppointmentsIcon.AssertSelected();
        }

        public void AssertAppointmentsNotSelected()
        {
            AppointmentsIcon.AssertNotSelected();
        }

        public void AssertPrescriptionsSelected()
        {
            PrescriptionsIcon.AssertSelected();
        }

        public void AssertYourHealthSelected()
        {
            YourHealthIcon.AssertSelected();
        }

        public void AssertMessagesSelected()
        {
            MessagesIcon.AssertSelected();
        }

        public void KeyboardNavigateToAdvice(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateIcon(AdviceIcon, navigation);

        public void KeyboardNavigateToAppointments(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateIcon(AppointmentsIcon, navigation);

        public void KeyboardNavigateToPrescriptions(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateIcon(PrescriptionsIcon, navigation);

        public void KeyboardNavigateToPrescriptionsFromElement(AndroidKeyboardNavigation navigation, IFocusable element)
            => KeyboardNavigateBetweenAndActivateIcon(PrescriptionsIcon, navigation, element);

        public void KeyboardNavigateToYourHealth(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateIcon(YourHealthIcon, navigation);

        public void KeyboardNavigateToMessages(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateIcon(MessagesIcon, navigation);

        public void KeyboardNavigateToHome(AndroidKeyboardNavigation navigation) =>
            KeyboardNavigateToAndActivateIcon(HomeIcon, navigation);

        public void KeyboardNavigateToHelp(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateIcon(HelpIcon, navigation);

        public void KeyboardNavigateToMore(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateIcon(MoreIcon, navigation);

        public void KeyboardNavigateToHomeFromElement(AndroidKeyboardNavigation navigation, IFocusable fromFocusable) =>
            KeyboardNavigateBetweenAndActivateIcon(HomeIcon, navigation, fromFocusable);

        private static void KeyboardNavigateBetweenAndActivateIcon(
            IFocusable icon,
            AndroidKeyboardNavigation keyboardPageContentNavigation,
            IFocusable fromFocusable)
        {
            keyboardPageContentNavigation.TabBetween(fromFocusable, icon);

            keyboardPageContentNavigation.PressEnterKey();
        }

        private static void KeyboardNavigateToAndActivateIcon(
            IFocusable icon,
            AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabTo(icon);

            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}
