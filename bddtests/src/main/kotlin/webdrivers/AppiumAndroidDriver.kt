package webdrivers

import config.Config
import io.appium.java_client.MobileElement
import io.appium.java_client.android.AndroidDriver
import io.appium.java_client.remote.MobileCapabilityType
import net.thucydides.core.webdriver.DriverSource
import org.openqa.selenium.WebDriver
import org.openqa.selenium.remote.DesiredCapabilities
import java.net.URL
import java.util.concurrent.TimeUnit

private const val TIMEOUT: Long = 5

class AppiumAndroidDriver : DriverSource{

    override fun newDriver(): WebDriver {
        val driver: AndroidDriver<MobileElement> = AndroidDriver(URL(Config.instance.appiumServer), caps())
        driver.manage().timeouts().implicitlyWait(TIMEOUT, TimeUnit.SECONDS)
        return driver
    }

    override fun takesScreenshots(): Boolean {
        return true
    }

    companion object {
        fun caps(): DesiredCapabilities {
            val caps = DesiredCapabilities()
            caps.setCapability(MobileCapabilityType.PLATFORM_NAME,"ANDROID")
            caps.setCapability(MobileCapabilityType.AUTOMATION_NAME,"Appium")
            caps.setCapability("autoWebview","true")
            caps.setCapability(MobileCapabilityType.APP,Config.instance.appPath)
            caps.setCapability(MobileCapabilityType.DEVICE_NAME,"Google Pixel 2")
            caps.setCapability(MobileCapabilityType.PLATFORM_VERSION,"8.0")

            return caps
        }
    }
}
