using System;
using System.Globalization;
using FluentAssertions;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Drivers.BrowserStack
{
    public sealed class IOSBrowserStackDriver : BrowserStackDriver<IOSDriver<IOSElement>, IOSElement>, IIOSBrowserStackDriver
    {
        public IOSBrowserStackDriver(Uri remoteAddress, AppiumOptions driverOptions)
            : base(new IOSDriver<IOSElement>(remoteAddress, driverOptions))
        {
            DateTime
                .Parse(Driver.DeviceTime, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal)
                .Should()
                .BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1), TestResultRetryExtensions.DeviceTimeSkewMessage);
        }
    }
}