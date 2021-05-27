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

        public IOSIcon AdviceIcon => FullNavigationFooter.ContainingButtonWithName("Advice");

        public IOSIcon AppointmentsIcon => FullNavigationFooter.ContainingButtonWithName("Appointments");

        public IOSIcon PrescriptionsIcon => FullNavigationFooter.ContainingButtonWithName("Prescriptions");

        public IOSIcon YourHealthIcon => FullNavigationFooter.ContainingButtonWithName("Your health");

        public IOSIcon MessagesIcon => FullNavigationFooter.ContainingButtonWithName("Messages");

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

        public void Home()
        {
            HomeIcon.Click();
        }

        public void More()
        {
            MoreIcon.Click();
        }

        public void Help()
        {
            HelpIcon.Click();
        }
    }
}