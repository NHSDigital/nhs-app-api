using System;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    // Temp as should be replaced by web logged in home page
    internal sealed class IOSLoggedInHomePage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSLoggedInHomePage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Message => new IOSLabel(_driver, "Logged In");

        internal static IOSLoggedInHomePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSLoggedInHomePage(driver);
            page.Message.AssertVisible(opts => opts.SetTimeout(TimeSpan.FromSeconds(10)));
            return page;
        }
    }
}