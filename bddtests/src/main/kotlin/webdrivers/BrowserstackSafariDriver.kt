package webdrivers

import config.Config
import net.thucydides.core.webdriver.DriverSource
import org.openqa.selenium.WebDriver
import org.openqa.selenium.safari.SafariOptions
import org.openqa.selenium.remote.RemoteWebDriver
import java.net.URL
import java.util.concurrent.TimeUnit

private const val TIMEOUT: Long = 5

class BrowserstackSafariDriver: DriverSource {

    override fun newDriver(): WebDriver {
        val driver = RemoteWebDriver(URL(Config.instance.browserstackUrl), caps())
        driver.manage().timeouts().implicitlyWait(TIMEOUT, TimeUnit.SECONDS)
        return driver
    }
    
    override fun takesScreenshots(): Boolean {
        return true
    }

    companion object {
        fun caps(): SafariOptions {
            val caps = SafariOptions()
            caps.setCapability("os", "OS X")
            caps.setCapability("os_version", "High Sierra")
            caps.setCapability("browser", "Safari")
            caps.setCapability("browserstack.local", "true")
            caps.setCapability("browserstack.debug", "true")
            caps.setCapability("browserstack.networkLogs", "true")
            caps.setCapability("browserstack.selenium_version", "3.5.2")
            caps.setCapability("browserstack.safari.enablePopups", "true")
            caps.setCapability("browserstack.safari.allowAllCookies", "true")

            if(Config.instance.browserstackLocalIdentifier!="")
                caps.setCapability("browserstack.localIdentifier",Config.instance.browserstackLocalIdentifier)

            if(Config.instance.browserstackBrowserResolution!="")
                caps.setCapability("resolution",Config.instance.browserstackBrowserResolution)

            if(Config.instance.browserstackTimezone!="")
                caps.setCapability("browserstack.timezone",Config.instance.browserstackTimezone)

             return caps
        }
    }

}
