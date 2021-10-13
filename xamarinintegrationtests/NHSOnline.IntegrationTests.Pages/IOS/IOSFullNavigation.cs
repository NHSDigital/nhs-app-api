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

        private IOSAppIcon HomeAppIcon => IOSAppIcon.WithName(_driver, "Home");

        private IOSAppIcon HelpAppIcon => IOSAppIcon.WithName(_driver, "Help");

        private IOSAppIcon MoreAppIcon => IOSAppIcon.WithName(_driver, "More");

        private IOSAppIcon AdviceAppIcon => IOSAppIcon.WithName(_driver, "Advice");

        private IOSAppIcon AppointmentsAppIcon => IOSAppIcon.WithName(_driver, "Appointments");

        private IOSAppIcon PrescriptionsAppIcon => IOSAppIcon.WithName(_driver, "Prescriptions");

        private IOSAppIcon YourHealthAppIcon => IOSAppIcon.WithName(_driver, "Your health");

        private IOSAppIcon MessagesAppIcon => IOSAppIcon.WithName(_driver, "Messages");

        internal void AssertNavigationIconsArePresent()
        {
            HomeAppIcon.AssertVisible();
            HelpAppIcon.AssertVisible();
            MoreAppIcon.AssertVisible();
            AdviceAppIcon.AssertVisible();
            AppointmentsAppIcon.AssertVisible();
            PrescriptionsAppIcon.AssertVisible();
            YourHealthAppIcon.AssertVisible();
            MessagesAppIcon.AssertVisible();
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