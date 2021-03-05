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


        private IOSIcon HomeIcon => FullNavigationHeader.ContainingIconWithDescription("NHS App home icon");

        private IOSIcon HelpIcon => FullNavigationHeader.ContainingIconWithDescription("NHS App help icon");

        private IOSIcon SettingsIcon => FullNavigationHeader.ContainingIconWithDescription("NHS App settings icon");

        private IOSNavigationMenuItem AdviceMenuItem => FullNavigationFooter.ContainingMenuItemWithDescriptionAndText("NHS App symptoms icon", "Symptoms");

        private IOSNavigationMenuItem AppointmentsMenuItem => FullNavigationFooter.ContainingMenuItemWithDescriptionAndText("NHS App appointments icon", "Appointments");

        private IOSNavigationMenuItem PrescriptionsMenuItem => FullNavigationFooter.ContainingMenuItemWithDescriptionAndText("NHS App prescriptions icon", "Prescriptions");

        private IOSNavigationMenuItem YourHealthMenuItem => FullNavigationFooter.ContainingMenuItemWithDescriptionAndText("NHS App records icon", "My Record");

        private IOSNavigationMenuItem MoreMenuItem => FullNavigationFooter.ContainingMenuItemWithDescriptionAndText("NHS App more icon", "More");

        private IOSNavigationMenuItem MessagesMenuItem => FullNavigationFooter.ContainingMenuItemWithDescriptionAndText("NHS App messages icon", "Messages");

        internal void AssertNavigationPresent()
        {
            FullNavigationHeader.AssertVisible();
            FullNavigationFooter.AssertVisible();
        }

        public void Home()
        {
            HomeIcon.Click();
        }

        public void Settings()
        {
            SettingsIcon.Click();
        }

        public void Advice()
        {
            AdviceMenuItem.Click();
        }

        public void Appointments()
        {
            AppointmentsMenuItem.Click();
        }

        public void Prescriptions()
        {
            PrescriptionsMenuItem.Click();
        }

        public void YourHealth()
        {
            YourHealthMenuItem.Click();
        }

        public void More()
        {
            MoreMenuItem.Click();
        }

        public void Help()
        {
            HelpIcon.Click();
        }
    }
}
