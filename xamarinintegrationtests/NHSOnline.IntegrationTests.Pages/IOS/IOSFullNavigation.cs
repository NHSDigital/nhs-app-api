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

        private IOSIcon HomeIcon => IOSIcon.WithDescription(_driver, "NHS App home icon");

        private IOSIcon HelpIcon => IOSIcon.WithDescription(_driver, "NHS App help icon");

        private IOSIcon SettingsIcon => IOSIcon.WithDescription(_driver, "NHS App settings icon");

        private IOSFullNavigationHeader FullNavigationHeader => IOSFullNavigationHeader.Create(_driver);

        private IOSFullNavigationFooter FullNavigationFooter => IOSFullNavigationFooter.Create(_driver);

        private IOSNavigationMenuItem AdviceMenuItem => IOSNavigationMenuItem.WithIconDescriptionAndText(_driver, "NHS App symptoms icon", "Symptoms");

        private IOSNavigationMenuItem AppointmentsMenuItem => IOSNavigationMenuItem.WithIconDescriptionAndText(_driver, "NHS App appointments icon", "Appointments");

        private IOSNavigationMenuItem PrescriptionsMenuItem => IOSNavigationMenuItem.WithIconDescriptionAndText(_driver, "NHS App prescriptions icon", "Prescriptions");

        private IOSNavigationMenuItem MyRecordMenuItem => IOSNavigationMenuItem.WithIconDescriptionAndText(_driver, "NHS App records icon", "My Record");

        private IOSNavigationMenuItem MoreMenuItem => IOSNavigationMenuItem.WithIconDescriptionAndText(_driver, "NHS App more icon", "More");

        private IOSNavigationMenuItem MessagesMenuItem => IOSNavigationMenuItem.WithIconDescriptionAndText(_driver, "NHS App messages icon", "Messages");

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

        public void MyRecord()
        {
            MyRecordMenuItem.Click();
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
