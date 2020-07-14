package webdrivers


import config.Config
import net.thucydides.core.webdriver.DriverSource
import org.openqa.selenium.WebDriver
import org.openqa.selenium.chrome.ChromeOptions
import org.openqa.selenium.remote.CapabilityType.SUPPORTS_JAVASCRIPT
import org.openqa.selenium.remote.RemoteWebDriver
import webdrivers.options.OptionManager
import webdrivers.options.nojs.NoJsOption
import java.net.URL
import java.util.concurrent.TimeUnit

private const val TIMEOUT: Long = 5

class BrowserstackChromeDriver : DriverSource {

    override fun newDriver(): WebDriver {
        val driver = RemoteWebDriver(URL(Config.instance.browserstackUrl), caps())
        driver.manage().timeouts().implicitlyWait(TIMEOUT, TimeUnit.SECONDS)

        return driver
    }

    override fun takesScreenshots(): Boolean {
        return true
    }

    companion object {
        fun caps(): ChromeOptions {
            val caps = ChromeOptions()
            caps.setCapability("os", "Windows")
            caps.setCapability("os_version", "10")
            caps.setCapability("browser", "Chrome")
            caps.setCapability("browser_version", "71.0")
            caps.setCapability("browserstack.local", "true")
            caps.setCapability("browserstack.debug", "true")
            caps.setCapability("browserstack.networkLogs", "true")
            caps.setCapability("browserstack.selenium_version", "3.5.2")

            if(Config.instance.browserstackLocalIdentifier!="")
                caps.setCapability("browserstack.localIdentifier",Config.instance.browserstackLocalIdentifier)

            if(Config.instance.browserstackBrowserResolution!="")
                caps.setCapability("resolution",Config.instance.browserstackBrowserResolution)

            if(Config.instance.browserstackTimezone!="")
                caps.setCapability("browserstack.timezone",Config.instance.browserstackTimezone)

            if (OptionManager.instance().isEnabled(NoJsOption::class)) {
                caps.setCapability(SUPPORTS_JAVASCRIPT, "false")
            }
            return caps
        }
    }
}
