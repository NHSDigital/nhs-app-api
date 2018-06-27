package webdrivers

import com.browserstack.local.Local
import config.Config
import net.thucydides.core.webdriver.DriverSource
import org.junit.Assert
import org.openqa.selenium.WebDriver
import org.openqa.selenium.remote.DesiredCapabilities
import org.openqa.selenium.remote.RemoteWebDriver
import java.net.URL

class BrowserstackChromeDriver : DriverSource {

    override fun newDriver(): WebDriver {
        val bsLocal = Local()
        val bsLocalArgs = HashMap<String, String>()
        bsLocalArgs.put("key", Config.instance.browserstackAccessKey)

        bsLocal.start(bsLocalArgs)
        Assert.assertTrue(bsLocal.isRunning)

        val url = URL(Config.instance.browserstackUrl)
        val caps: DesiredCapabilities = BrowserstackChromeDriver.caps()

        return RemoteWebDriver(url, caps)
    }

    override fun takesScreenshots(): Boolean {
        return true
    }

    companion object {
        fun caps(): DesiredCapabilities {
            val caps = DesiredCapabilities()
            caps.setCapability("browser", "Chrome")
            caps.setCapability("browser_version", "66.0")
            caps.setCapability("os", "Windows")
            caps.setCapability("os_version", "10")
            caps.setCapability("resolution", "1024x768")
            caps.setCapability("browserstack.local", "true")

            return caps
        }
    }
}