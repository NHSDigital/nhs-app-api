package webdrivers

import config.Config
import io.appium.java_client.android.AndroidDriver
import io.appium.java_client.remote.AndroidMobileCapabilityType
import io.appium.java_client.remote.MobileCapabilityType
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.webdriver.DriverSource
import org.openqa.selenium.WebDriver
import org.openqa.selenium.remote.DesiredCapabilities
import utils.GlobalSerenityHelpers
import utils.getOrFail
import utils.set
import java.net.URL
import java.util.concurrent.TimeUnit

private const val TIMEOUT: Long = 5
private const val DEFAULT_DEVICE_NAME = "Google Pixel 2"
private const val DEFAULT_OS_VERSION = "8.0"

class BrowserstackAndroidDriver : DriverSource {

    override fun newDriver(): WebDriver {
        GlobalSerenityHelpers.APP_BUNDLE_ID.set("com.nhs.online.nhsonline")

        val driver: AndroidDriver<WebElementFacade> = AndroidDriver(URL(Config.instance.browserstackUrl), caps())
        driver.manage().timeouts().implicitlyWait(TIMEOUT, TimeUnit.SECONDS)
        driver.unlockDevice()
        return driver
    }

    override fun takesScreenshots(): Boolean {
        return true
    }

    companion object {
        fun caps(): DesiredCapabilities {
            val caps = DesiredCapabilities()

            caps.setCapability("autoWebview", true)
            caps.setCapability(AndroidMobileCapabilityType.AUTO_GRANT_PERMISSIONS, true)
            caps.setCapability(AndroidMobileCapabilityType.NATIVE_WEB_SCREENSHOT, true)
            caps.setCapability(AndroidMobileCapabilityType.DONT_STOP_APP_ON_RESET, true)

            caps.setCapability("project", "NHSApp")
            caps.setCapability("build", Config.instance.browserstackBuild)
            caps.setCapability("name", GlobalSerenityHelpers.SCENARIO_TITLE.getOrFail<String>())

            caps.setCapability(MobileCapabilityType.APP, Config.instance.appPath)
            caps.setCapabilityIfNotNull("browserstack.app_version", Config.instance.browserstackAppVersion)

            caps.setCapability(
                    MobileCapabilityType.DEVICE_NAME,
                    Config.instance.browserstackDeviceName ?: DEFAULT_DEVICE_NAME)
            caps.setCapability(
                    "os_version",
                    Config.instance.browserstackDeviceOSVersion ?: DEFAULT_OS_VERSION)

            caps.setCapabilityIfNotNull("browserstack.localIdentifier", Config.instance.browserstackLocalIdentifier)
            caps.setCapabilityIfNotNull("browserstack.networkProfile", Config.instance.browserstackNetworkProfile)
            caps.setCapability("browserstack.local", "true")
            caps.setCapability("browserstack.debug","true")

            return caps
        }
    }
}
