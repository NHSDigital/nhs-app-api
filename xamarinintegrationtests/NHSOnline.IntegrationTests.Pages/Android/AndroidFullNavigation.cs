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

        private AndroidFullNavigationHeader FullNavigationHeader => new AndroidFullNavigationHeader(_driver);

        private AndroidFullNavigationFooter FullNavigationFooter => new AndroidFullNavigationFooter(_driver);


        private AndroidIcon HomeIcon => new AndroidIcon(_driver, "NHS App home icon");

        private AndroidIcon HelpIcon => new AndroidIcon(_driver, "NHS App help icon");

        private AndroidIcon SettingsIcon => new AndroidIcon(_driver, "NHS App settings icon");

        private AndroidNavigationMenuItem SymptomsMenuItem => new AndroidNavigationMenuItem(_driver, "NHS App symptoms icon", "Symptoms");

        private AndroidNavigationMenuItem AppointmentsMenuItem => new AndroidNavigationMenuItem(_driver, "NHS App appointments icon", "Appointments");

        private AndroidNavigationMenuItem PrescriptionsMenuItem => new AndroidNavigationMenuItem(_driver, "NHS App prescriptions icon", "Prescriptions");

        private AndroidNavigationMenuItem MyRecordMenuItem => new AndroidNavigationMenuItem(_driver, "NHS App records icon", "My Record");

        private AndroidNavigationMenuItem MoreMenuItem => new AndroidNavigationMenuItem(_driver, "NHS App more icon", "More");

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
