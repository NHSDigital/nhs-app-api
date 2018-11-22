package webdrivers

import config.Config
import io.appium.java_client.MobileElement
import io.appium.java_client.ios.IOSDriver
import io.appium.java_client.remote.IOSMobileCapabilityType
import io.appium.java_client.remote.MobileCapabilityType
import net.thucydides.core.webdriver.DriverSource
import org.openqa.selenium.WebDriver
import org.openqa.selenium.remote.DesiredCapabilities
import java.net.URL
import java.util.concurrent.TimeUnit

private const val TIMEOUT: Long = 5

class IPhoneXDriver : DriverSource {

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
            //connect to browser stack on cloud
            caps.setCapability(MobileCapabilityType.DEVICE_NAME, "iPhone X")
            caps.setCapability(IOSMobileCapabilityType.ACCEPT_SSL_CERTS, true)
            caps.setCapability("browserstack.local", "true")
            caps.setCapability("autoWebview","true")
            caps.setCapability(MobileCapabilityType.APP,Config.instance.appPath)
            if(Config.instance.browserstackLocalIdentifier!="")
                caps.setCapability("browserstack.localIdentifier",Config.instance.browserstackLocalIdentifier)

            return caps
        }
    }
}