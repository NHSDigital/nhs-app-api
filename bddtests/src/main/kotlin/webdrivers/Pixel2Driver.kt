package webdrivers

import config.Config
import io.appium.java_client.android.AndroidDriver
import io.appium.java_client.remote.AndroidMobileCapabilityType
import io.appium.java_client.remote.MobileCapabilityType
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.webdriver.DriverSource
import org.junit.Assert
import org.openqa.selenium.WebDriver
import org.openqa.selenium.remote.DesiredCapabilities
import webdrivers.browserstack.BrowserstackLocalService
import java.net.URL
import java.util.concurrent.TimeUnit

class Pixel2Driver : DriverSource {

    override fun newDriver(): WebDriver {
        val driver: AndroidDriver<WebElementFacade> = AndroidDriver(URL(Config.instance.browserstackUrl), caps())
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
            caps.setCapability(MobileCapabilityType.DEVICE_NAME, "Google Pixel 2")
            caps.setCapability("os_version", "8.0")
            caps.setCapability("autoWebview", true)
            caps.setCapability(AndroidMobileCapabilityType.AUTO_GRANT_PERMISSIONS, true)
            caps.setCapability(AndroidMobileCapabilityType.NATIVE_WEB_SCREENSHOT, true)
            caps.setCapability(AndroidMobileCapabilityType.ANDROID_SCREENSHOT_PATH, "target/screenshots")
            caps.setCapability("browserstack.local", "true")

            return caps
        }
    }
}