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

        private AndroidNavigationBar FullNavigationHeader => AndroidNavigationBar.WithName(_driver, "NHS App Full Navigation Header");

        private AndroidNavigationBar FullNavigationFooter => AndroidNavigationBar.WithName(_driver, "NHS App Full Navigation Footer");

        private AndroidIcon HomeIcon => FullNavigationHeader.ContainingIconWithName("Home");

        private AndroidIcon HelpIcon => FullNavigationHeader.ContainingIconWithName("Help");

        private AndroidIcon MoreIcon => FullNavigationHeader.ContainingIconWithName("More");

        private AndroidIcon AdviceIcon => FullNavigationFooter.ContainingIconWithName("Advice");

        private AndroidIcon AppointmentsIcon => FullNavigationFooter.ContainingIconWithName("Appointments");

        private AndroidIcon PrescriptionsIcon => FullNavigationFooter.ContainingIconWithName("Prescriptions");

        private AndroidIcon YourHealthIcon => FullNavigationFooter.ContainingIconWithName("Your health");

        private AndroidIcon MessagesIcon => FullNavigationFooter.ContainingIconWithName("Messages");

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


        public void AssertNavigationPresent()
        {
            FullNavigationHeader.AssertVisible();
            FullNavigationFooter.AssertVisible();
        }

        public void Home()
        {
            HomeIcon.Click();
        }

        public void Messages()
        {
            MessagesIcon.Click();
        }

        public void Advice()
        {
            AdviceIcon.Click();
        }

        public void Appointments()
        {
            AppointmentsIcon.Click();
        }

        public void Prescriptions()
        {
            PrescriptionsIcon.Click();
        }

        public void YourHealth()
        {
            YourHealthIcon.Click();
        }

        public void More()
        {
            MoreIcon.Click();
        }

        public void Help()
        {
            HelpIcon.Click();
        }

        public void KeyboardNavigateToAdvice(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateIcon(AdviceIcon, navigation);

        public void KeyboardNavigateToAppointments(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateIcon(AppointmentsIcon, navigation);

        public void KeyboardNavigatePrescriptions(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateIcon(PrescriptionsIcon, navigation);

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
