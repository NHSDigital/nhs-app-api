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

        private IOSAppIcon HomeAppIcon => FullNavigationHeader.ContainingButtonWithName("Home");

        private IOSAppIcon HelpAppIcon => FullNavigationHeader.ContainingButtonWithName("Help");

        private IOSAppIcon MoreAppIcon => FullNavigationHeader.ContainingButtonWithName("More");

        private IOSAppIcon AdviceAppIcon => FullNavigationFooter.ContainingButtonWithName("Advice");

        private IOSAppIcon AppointmentsAppIcon => FullNavigationFooter.ContainingButtonWithName("Appointments");

        private IOSAppIcon PrescriptionsAppIcon => FullNavigationFooter.ContainingButtonWithName("Prescriptions");

        private IOSAppIcon YourHealthAppIcon => FullNavigationFooter.ContainingButtonWithName("Your health");

        private IOSAppIcon MessagesAppIcon => FullNavigationFooter.ContainingButtonWithName("Messages");

        internal void AssertNavigationPresent()
        {
            FullNavigationHeader.AssertVisible();
            FullNavigationFooter.AssertVisible();
        }

        public void AssertNoIconsSelected()
        {
            AdviceAppIcon.AssertNotSelected();
            AppointmentsAppIcon.AssertNotSelected();
            YourHealthAppIcon.AssertNotSelected();
            MessagesAppIcon.AssertNotSelected();
            PrescriptionsAppIcon.AssertNotSelected();
        }

        public void NavigateToHome()
        {
            HomeAppIcon.Click();
        }

        public void NavigateToMore()
        {
            MoreAppIcon.Click();
        }

        public void NavigateToHelp()
        {
            HelpAppIcon.Click();
        }

        public void NavigateToAdvice()
        {
            AdviceAppIcon.Click();
        }

        public void NavigateToAppointments()
        {
            AppointmentsAppIcon.Click();
        }

        public void NavigateToPrescriptions()
        {
            PrescriptionsAppIcon.Click();
        }

        public void NavigateToYourHealth()
        {
            YourHealthAppIcon.Click();
        }

        public void NavigateToMessages()
        {
            MessagesAppIcon.Click();
        }

        public void AssertNoNavigationIconsSelected()
        {
            AdviceAppIcon.AssertNotSelected();
            AppointmentsAppIcon.AssertNotSelected();
            PrescriptionsAppIcon.AssertNotSelected();
            YourHealthAppIcon.AssertNotSelected();
            MessagesAppIcon.AssertNotSelected();
        }

        public void AssertAdviceSelected()
        {
            AdviceAppIcon.AssertSelected();
        }

        public void AssertAppointmentsSelected()
        {
            AppointmentsAppIcon.AssertSelected();
        }

        public void AssertAppointmentsNotSelected()
        {
            AppointmentsAppIcon.AssertNotSelected();
        }

        public void AssertPrescriptionsSelected()
        {
            PrescriptionsAppIcon.AssertSelected();
        }

        public void AssertYourHealthSelected()
        {
            YourHealthAppIcon.AssertSelected();
        }

        public void AssertMessagesSelected()
        {
            MessagesAppIcon.AssertSelected();
        }
    }
}