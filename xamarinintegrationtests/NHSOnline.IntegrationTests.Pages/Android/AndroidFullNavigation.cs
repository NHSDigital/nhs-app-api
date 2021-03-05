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


        private AndroidIcon HomeIcon => FullNavigationHeader.ContainingIconWithDescription("NHS App home icon");

        private AndroidIcon HelpIcon => FullNavigationHeader.ContainingIconWithDescription("NHS App help icon");

        private AndroidIcon SettingsIcon => FullNavigationHeader.ContainingIconWithDescription("NHS App settings icon");

        private AndroidNavigationMenuItem AdviceMenuItem => FullNavigationFooter.ContainingMenuItemWithDescriptionAndText("NHS App symptoms icon", "Symptoms");

        private AndroidNavigationMenuItem AppointmentsMenuItem => FullNavigationFooter.ContainingMenuItemWithDescriptionAndText("NHS App appointments icon", "Appointments");

        private AndroidNavigationMenuItem PrescriptionsMenuItem => FullNavigationFooter.ContainingMenuItemWithDescriptionAndText("NHS App prescriptions icon", "Prescriptions");

        private AndroidNavigationMenuItem YourHealthMenuItem => FullNavigationFooter.ContainingMenuItemWithDescriptionAndText("NHS App records icon", "My Record");

        private AndroidNavigationMenuItem MoreMenuItem => FullNavigationFooter.ContainingMenuItemWithDescriptionAndText("NHS App more icon", "More");

        private AndroidNavigationMenuItem MessagesMenuItem => FullNavigationFooter.ContainingMenuItemWithDescriptionAndText("NHS App messages icon", "Messages");

        public void AssertNavigationPresent()
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
