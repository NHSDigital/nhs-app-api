using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSFullNavigation
    {
        private readonly IIOSDriverWrapper _driver;

        internal IOSFullNavigation(IIOSDriverWrapper driver)
        {
            _driver = driver;
        }

        private IOSNavigationBar FullNavigationHeader => IOSNavigationBar.WithName(_driver, "NHS App Full Navigation Header");

        private IOSNavigationBar FullNavigationFooter => IOSNavigationBar.WithName(_driver, "NHS App Full Navigation Footer");

        private IOSIcon HomeIcon => FullNavigationHeader.ContainingButtonWithName("Home");

        private IOSIcon HelpIcon => FullNavigationHeader.ContainingButtonWithName("Help");

        private IOSIcon MoreIcon => FullNavigationHeader.ContainingButtonWithName("More");

        private IOSIcon AdviceIcon => FullNavigationFooter.ContainingButtonWithName("Advice");

        private IOSIcon AppointmentsIcon => FullNavigationFooter.ContainingButtonWithName("Appointments");

        private IOSIcon PrescriptionsIcon => FullNavigationFooter.ContainingButtonWithName("Prescriptions");

        private IOSIcon YourHealthIcon => FullNavigationFooter.ContainingButtonWithName("Your health");

        private IOSIcon MessagesIcon => FullNavigationFooter.ContainingButtonWithName("Messages");

        internal void AssertNavigationPresent()
        {
            FullNavigationHeader.AssertVisible();
            FullNavigationFooter.AssertVisible();
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

        public void NavigateToMore()
        {
            MoreIcon.Click();
        }

        public void NavigateToHelp()
        {
            HelpIcon.Click();
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
    }
}