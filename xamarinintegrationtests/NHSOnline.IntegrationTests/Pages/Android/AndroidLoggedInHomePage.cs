using System;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    // Temp as should be replaced by web logged in home page
    internal sealed class AndroidLoggedInHomePage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidLoggedInHomePage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Message => new AndroidLabel(_driver, "Logged In");

        internal static AndroidLoggedInHomePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidLoggedInHomePage(driver);
            page.Message.AssertVisible(opts => opts.SetTimeout(TimeSpan.FromSeconds(10)));
            return page;
        }
    }
}