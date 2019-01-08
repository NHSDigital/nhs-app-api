package webdrivers

import config.Config
import io.appium.java_client.MobileElement
import io.appium.java_client.ios.IOSDriver
import io.appium.java_client.remote.MobileCapabilityType
import net.thucydides.core.webdriver.DriverSource
import org.openqa.selenium.WebDriver
import org.openqa.selenium.remote.DesiredCapabilities
import java.net.URL
import java.util.concurrent.TimeUnit

private const val TIMEOUT: Long = 5

class AppiumIOSDriver : DriverSource{

    override fun newDriver(): WebDriver {
        val driver: IOSDriver<MobileElement> = IOSDriver(URL(Config.instance.appiumServer), caps())
        driver.manage().timeouts().implicitlyWait(TIMEOUT, TimeUnit.SECONDS)
        return driver
    }

    override fun takesScreenshots(): Boolean {
        return true
    }

    companion object {
        fun caps(): DesiredCapabilities {
            val caps = DesiredCapabilities()

            caps.setCapability(MobileCapabilityType.PLATFORM_NAME,"iOS")
            caps.setCapability(MobileCapabilityType.AUTOMATION_NAME,"XCUITest")
            caps.setCapability("autoWebview","true")
            caps.setCapability(MobileCapabilityType.APP,Config.instance.appPath)
            //below capability can be set for any device name returned by
            //'instruments -s device' which returns a list of all known IOS devices - real and simulators
            caps.setCapability(MobileCapabilityType.DEVICE_NAME,"iPhone 8")
            //below capability has to be set if you have device profiles for multiple os versions
            caps.setCapability(MobileCapabilityType.PLATFORM_VERSION,"12.1")

            return caps
        }
    }
}
