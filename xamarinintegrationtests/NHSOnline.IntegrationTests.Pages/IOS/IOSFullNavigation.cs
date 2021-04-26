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

        public void Home()
        {
            HomeIcon.ClickButton();
        }

        public void Advice()
        {
            AdviceIcon.ClickButton();
        }

        public void Appointments()
        {
            AppointmentsIcon.ClickButton();
        }

        public void Prescriptions()
        {
            PrescriptionsIcon.ClickButton();
        }

        public void YourHealth()
        {
            YourHealthIcon.ClickButton();
        }

        public void Messages()
        {
            MessagesIcon.ClickButton();
        }

        public void More()
        {
            MoreIcon.ClickButton();
        }

        public void Help()
        {
            HelpIcon.ClickButton();
        }
    }
}