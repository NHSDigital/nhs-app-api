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

private const val TIMEOUT: Long = 5

class BrowserstackAndroidDriver : DriverSource {

    override fun newDriver(): WebDriver {
        val driver: AndroidDriver<WebElementFacade> = AndroidDriver(URL(Config.instance.browserstackUrl), caps())
        driver.manage().timeouts().implicitlyWait(TIMEOUT, TimeUnit.SECONDS)
        return driver
    }

    override fun takesScreenshots(): Boolean {
        return true
    }

    companion object {
        fun caps(): DesiredCapabilities {
            val caps = DesiredCapabilities()
            caps.setCapability(MobileCapabilityType.APP, Config.instance.appPath)
            caps.setCapability("autoWebview", true)
            caps.setCapability(AndroidMobileCapabilityType.AUTO_GRANT_PERMISSIONS, true)
            caps.setCapability(AndroidMobileCapabilityType.NATIVE_WEB_SCREENSHOT, true)
            caps.setCapability("browserstack.local", "true")
            caps.setCapability("browserstack.debug","true")
            caps.setCapability("project", "NHSApp")
            caps.setCapability("build", Config.instance.browserstackBuild)

            if(Config.instance.browserstackLocalIdentifier!="")
                caps.setCapability("browserstack.localIdentifier",Config.instance.browserstackLocalIdentifier)

            if(Config.instance.browserstackDeviceName!="")
                caps.setCapability(MobileCapabilityType.DEVICE_NAME,Config.instance.browserstackDeviceName)
            else
                caps.setCapability(MobileCapabilityType.DEVICE_NAME,"Google Pixel 2")

            if(Config.instance.browserstackDeviceOSversion!="")
                caps.setCapability("os_version",Config.instance.browserstackDeviceOSversion)
            else
                caps.setCapability("os_version","8.0")

            if(Config.instance.browserstackaAppVersion!="")
               caps.setCapability("browserstack.app_version",Config.instance.browserstackaAppVersion)

            if(Config.instance.browserstackNetworkProfile!="")
                caps.setCapability("browserstack.networkProfile",Config.instance.browserstackNetworkProfile)

            return caps
        }
    }
}