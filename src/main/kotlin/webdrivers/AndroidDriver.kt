package webdrivers

import config.Config
import io.appium.java_client.android.AndroidDriver
import io.appium.java_client.remote.AndroidMobileCapabilityType
import io.appium.java_client.remote.MobileCapabilityType
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.webdriver.DriverSource
import org.openqa.selenium.WebDriver
import org.openqa.selenium.remote.DesiredCapabilities
import java.net.URL
import java.util.concurrent.TimeUnit

class AndroidDriver : DriverSource {

    override fun newDriver(): WebDriver {
        var caps = DesiredCapabilities()

        caps.setCapability(MobileCapabilityType.APP, Config.instance.appPath)
        caps.setCapability(MobileCapabilityType.DEVICE_NAME, "Google Pixel 2")
        caps.setCapability(MobileCapabilityType.PLATFORM_VERSION, "8.1.0")
        caps.setCapability(AndroidMobileCapabilityType.AUTO_GRANT_PERMISSIONS, true)
        caps.setCapability(AndroidMobileCapabilityType.NATIVE_WEB_SCREENSHOT, true)
        caps.setCapability(AndroidMobileCapabilityType.ANDROID_SCREENSHOT_PATH, "target/screenshots")

        val driver = AndroidDriver<WebElementFacade>(URL(Config.instance.appiumServer), caps)
        driver.manage().timeouts().implicitlyWait(15, TimeUnit.SECONDS)
        return driver
    }

    override fun takesScreenshots(): Boolean {
        return true
    }
}
