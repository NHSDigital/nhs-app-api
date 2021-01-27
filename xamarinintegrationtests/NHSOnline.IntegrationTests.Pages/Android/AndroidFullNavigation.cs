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

        private AndroidFullNavigationHeader FullNavigationHeader => AndroidFullNavigationHeader.Create(_driver);

        private AndroidFullNavigationFooter FullNavigationFooter => AndroidFullNavigationFooter.Create(_driver);


        private AndroidIcon HomeIcon => AndroidIcon.WithDescription(_driver, "NHS App home icon");

        private AndroidIcon HelpIcon => AndroidIcon.WithDescription(_driver, "NHS App help icon");

        private AndroidIcon SettingsIcon => AndroidIcon.WithDescription(_driver, "NHS App settings icon");

        private AndroidNavigationMenuItem SymptomsMenuItem => AndroidNavigationMenuItem.WithIconDescriptionAndText(_driver, "NHS App symptoms icon", "Symptoms");

        private AndroidNavigationMenuItem AppointmentsMenuItem => AndroidNavigationMenuItem.WithIconDescriptionAndText(_driver, "NHS App appointments icon", "Appointments");

        private AndroidNavigationMenuItem PrescriptionsMenuItem => AndroidNavigationMenuItem.WithIconDescriptionAndText(_driver, "NHS App prescriptions icon", "Prescriptions");

        private AndroidNavigationMenuItem MyRecordMenuItem => AndroidNavigationMenuItem.WithIconDescriptionAndText(_driver, "NHS App records icon", "My Record");

        private AndroidNavigationMenuItem MoreMenuItem => AndroidNavigationMenuItem.WithIconDescriptionAndText(_driver, "NHS App more icon", "More");

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

        public void Symptoms()
        {
            SymptomsMenuItem.Click();
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
