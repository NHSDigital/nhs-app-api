package webdrivers

import config.Config
import io.appium.java_client.ios.IOSDriver
import io.appium.java_client.remote.IOSMobileCapabilityType
import io.appium.java_client.remote.MobileCapabilityType
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.webdriver.DriverSource
import org.junit.Assert
import org.openqa.selenium.WebDriver
import org.openqa.selenium.remote.DesiredCapabilities
import webdrivers.browserstack.BrowserstackLocalService
import java.net.URL
import java.util.concurrent.TimeUnit

class IPhoneXDriver : DriverSource {

    override fun newDriver(): WebDriver {
        val driver: IOSDriver<WebElementFacade> = IOSDriver(URL(Config.instance.browserstackUrl), caps())
        driver.manage().timeouts().implicitlyWait(5, TimeUnit.SECONDS)
        return driver
    }

    override fun takesScreenshots(): Boolean {
        return true
    }

    companion object {
        fun caps(): DesiredCapabilities {
            val caps = DesiredCapabilities()
            caps.setCapability(MobileCapabilityType.APP, Config.instance.appPath)
            caps.setCapability(MobileCapabilityType.DEVICE_NAME, "iPhone X")
            caps.setCapability(IOSMobileCapabilityType.ACCEPT_SSL_CERTS, true)
            caps.setCapability("browserstack.local", "true")

            return caps
        }
    }
}