package webdrivers

import config.Config
import io.appium.java_client.MobileElement
import io.appium.java_client.ios.IOSDriver
import io.appium.java_client.remote.IOSMobileCapabilityType
import io.appium.java_client.remote.MobileCapabilityType
import net.thucydides.core.webdriver.DriverSource
import org.openqa.selenium.WebDriver
import org.openqa.selenium.remote.DesiredCapabilities
import utils.GlobalSerenityHelpers
import utils.getOrFail
import java.net.URL
import java.util.concurrent.TimeUnit

private const val TIMEOUT: Long = 5
private const val DEFAULT_DEVICE_NAME = "iPhone 8"
private const val DEFAULT_OS_VERSION = "12.1"

class BrowserstackIOSDriver : DriverSource {

    override fun newDriver(): WebDriver {
        val driver: IOSDriver<MobileElement> = IOSDriver(URL(Config.instance.browserstackUrl), caps())
        driver.manage().timeouts().implicitlyWait(TIMEOUT, TimeUnit.SECONDS)
        return driver
    }

    override fun takesScreenshots(): Boolean {
        return true
    }

    companion object {
        fun caps(): DesiredCapabilities {
            val caps = DesiredCapabilities()

            caps.setCapability("autoWebview", "true")
            caps.setCapability(IOSMobileCapabilityType.ACCEPT_SSL_CERTS, true)

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
                    Config.instance.browserstackDeviceOSversion ?: DEFAULT_OS_VERSION)

            caps.setCapabilityIfNotNull("browserstack.localIdentifier", Config.instance.browserstackLocalIdentifier)
            caps.setCapabilityIfNotNull("browserstack.networkProfile", Config.instance.browserstackNetworkProfile)
            caps.setCapability("browserstack.local", "true")
            caps.setCapability("browserstack.debug","true")

            return caps
        }
    }
}